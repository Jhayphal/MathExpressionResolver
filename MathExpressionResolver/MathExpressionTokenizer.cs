using System;
using System.Linq;
using System.Collections.Generic;

namespace MathExpressionResolver
{
  internal class MathExpressionTokenizer : ITokenizer<MathExpressionTokenType>
  {
    private const string DecimalSeparator = ".";
    private const string Exponent = "e";

    private readonly Tokenizer<MathExpressionTokenType> tokenizer;
    private readonly Stack<string> currentNumber = new Stack<string>();

    private MathExpressionTokenType context = MathExpressionTokenType.Unknown;

    public MathExpressionTokenizer(SupportedOperations supportedOperations)
    {
      Operations = supportedOperations;

      var operators = supportedOperations
        .Where(o => o is IOperator)
        .Select(o => o.Operator);

      var functions = supportedOperations
        .Where(o => !(o is IOperator))
        .Select(o => o.Operator);

      var numbers = Enumerable.Range(0, 10)
        .Select(n => n.ToString())
        .Concat(new string[] { Exponent, DecimalSeparator });
      
      var tokens = new Dictionary<MathExpressionTokenType, HashSet<string>>
      {
        { MathExpressionTokenType.Operator, new HashSet<string>(operators) },
        { MathExpressionTokenType.Function, new HashSet<string>(functions) },
        { MathExpressionTokenType.OpenBracket, new HashSet<string> { "(" } },
        { MathExpressionTokenType.CloseBracket, new HashSet<string> { ")" } },
        { MathExpressionTokenType.Number, new HashSet<string>(numbers) }
      };

      tokenizer = new Tokenizer<MathExpressionTokenType>(tokens, caseSensetive: false);
    }

    public readonly SupportedOperations Operations;

    public virtual IEnumerable<(MathExpressionTokenType Type, string Value)> GetTokens(string expression)
    {
      foreach (var current in tokenizer.GetTokens(expression))
      {
        foreach(var result in ProccessToken(current, context))
        {
          yield return result;
        }
      }

      if (HasNumber())
      {
        yield return PopNumber();
      }

      context = MathExpressionTokenType.Unknown;
    }

    private IEnumerable<(MathExpressionTokenType Type, string Value)> ProccessToken((MathExpressionTokenType Type, string Token) current, MathExpressionTokenType context)
    {
      IEnumerable<(MathExpressionTokenType Type, string Value)> output;

      switch (context)
      {
        case MathExpressionTokenType.Unknown:
          output = ProccessUnknownContext(current);
          break;
        case MathExpressionTokenType.Number:
          output = ProccessNumberContext(current);
          break;
        case MathExpressionTokenType.Operator:
          output = ProccessOperatorContext(current);
          break;
        case MathExpressionTokenType.OpenBracket:
          output = ProccessOpenBracketContext(current);
          break;
        case MathExpressionTokenType.CloseBracket:
          output = ProccessCloseBracketContext(current);
          break;
        case MathExpressionTokenType.Function:
          output = ProccessFunctionContext(current);
          break;
        default:
          throw new InvalidOperationException(context.ToString());
      }

      foreach (var result in output)
      {
        yield return result;
      }
    }

    private IEnumerable<(MathExpressionTokenType Type, string Value)> ProccessUnknownContext((MathExpressionTokenType Type, string Value) token)
    {
      if (HasNumber())
      {
        throw new ArgumentException("Wrong context - number should be empty in unknown context");
      }

      switch (token.Type)
      {
        case MathExpressionTokenType.Operator:
          if (IsSign(token.Value))
          {
            context = MathExpressionTokenType.Number;
            PushNumber(token.Value);
          }
          else
          {
            throw new ArgumentException($"Unexpected operator '{token.Value}' in unknown context");
          }
          break;

        case MathExpressionTokenType.Number:
          if (token.Value == DecimalSeparator || IsExponent(token.Value))
          {
            throw new ArgumentException("Number should beginning at a sign or digit");
          }

          context = MathExpressionTokenType.Number;
          PushNumber(token.Value);
          break;

        case MathExpressionTokenType.OpenBracket:
          context = MathExpressionTokenType.Unknown;
          yield return token;
          break;

        case MathExpressionTokenType.Function:
          context = MathExpressionTokenType.Function;
          yield return token;
          break;

        default:
          throw new ArgumentException($"Unexpected token type '{token.Type}' in unknown context");
      }
    }

    private IEnumerable<(MathExpressionTokenType Type, string Value)> ProccessNumberContext((MathExpressionTokenType Type, string Value) token)
    {
      if (!HasNumber())
      {
        throw new ArgumentException("Number cannot be empty in number context");
      }

      string sign;

      switch (token.Type)
      {
        case MathExpressionTokenType.Number:
          if (token.Value == DecimalSeparator)
          {
            if (currentNumber.Contains(DecimalSeparator))
            {
              throw new ArgumentException("Decimal separator duplicates");
            }
            else if (currentNumber.Contains(Exponent))
            {
              throw new ArgumentException("Both decimal separator and exponent literals in one number");
            }
            else
            {
              var lastLiteral = currentNumber.Peek();

              if (!char.IsDigit(lastLiteral[0]))
              {
                throw new ArgumentException($"Invalid number. Decimal separator after '{lastLiteral}' literal");
              }
            }
          }
          else if (IsExponent(token.Value))
          {
            if (currentNumber.Contains(Exponent))
            {
              throw new ArgumentException("Exponent duplicates");
            }
            else
            {
              var lastLiteral = currentNumber.Peek();

              if (!char.IsDigit(lastLiteral[0]))
              {
                throw new ArgumentException($"Invalid number. Exponent after '{lastLiteral}' literal");
              }
            }
          }
          
          context = MathExpressionTokenType.Number;
          PushNumber(token.Value);
          break;

        case MathExpressionTokenType.Operator:
          if (IsSign(token.Value) && IsExponent(currentNumber.Peek()))
          {
            context = MathExpressionTokenType.Number;
            PushNumber(token.Value);
          }
          else if (IsSign(token.Value)
            && !currentNumber.Contains(Exponent)
            && IsSign(currentNumber.Peek())
            && currentNumber.Count == 1)
          {
            if (token.Value != "-")
            {
              throw new ArgumentException($"Unexpected sign '{token.Type}'");
            }

            sign = currentNumber.Pop();
            currentNumber.Push(sign == "+" ? "-" : "+");
          }
          else
          {
            yield return PopNumber();
            context = MathExpressionTokenType.Operator;
            yield return token;
          }
          break;

        case MathExpressionTokenType.CloseBracket:
          yield return PopNumber();
          context = MathExpressionTokenType.CloseBracket;
          yield return token;
          break;

        case MathExpressionTokenType.OpenBracket:
          // in expression like '8 * -(1 + 1)', when 'Operator -=> (Number sign or OpenBracket sign)'
          // number context expected by default
          if (!TryGetSign(out sign))
          {
            throw new ArgumentException($"Unexpected literal '{token.Value}' in number context");
          }

          if (sign == "-")
          {
            yield return (MathExpressionTokenType.Number, "-1");
            yield return (MathExpressionTokenType.Operator, "*");
          }

          context = MathExpressionTokenType.Unknown;
          yield return token;
          break;

        default:
          throw new ArgumentException($"Unexpected token type '{token.Type}' in number context");
      }
    }

    private IEnumerable<(MathExpressionTokenType Type, string Value)> ProccessOperatorContext((MathExpressionTokenType Type, string Value) token)
    {
      switch (token.Type)
      {
        case MathExpressionTokenType.Number:
          context = MathExpressionTokenType.Number;
          PushNumber(token.Value);
          break;

        case MathExpressionTokenType.Operator:
          if (!IsSign(token.Value))
          {
            throw new ArgumentException($"Unexpected literal '{token.Value}' in operator context");
          }

          context = MathExpressionTokenType.Number;
          PushNumber(token.Value);
          break;

        case MathExpressionTokenType.OpenBracket:
          context = MathExpressionTokenType.Unknown;
          yield return token;
          break;

        case MathExpressionTokenType.Function:
          context = MathExpressionTokenType.Function;
          yield return token;
          break;

        default:
          throw new ArgumentException($"Unexpected token type '{token.Type}' in operator context");
      }
    }

    private IEnumerable<(MathExpressionTokenType Type, string Value)> ProccessOpenBracketContext((MathExpressionTokenType Type, string Value) token)
    {
      throw new ArgumentException($"Unsupported open bracket context");
    }

    private IEnumerable<(MathExpressionTokenType Type, string Value)> ProccessCloseBracketContext((MathExpressionTokenType Type, string Value) token)
    {
      switch (token.Type)
      {
        case MathExpressionTokenType.Operator:
        case MathExpressionTokenType.CloseBracket:
          context = token.Type;
          yield return token;
          break;

        default:
          throw new ArgumentException($"Unexpected token type '{token.Type}' in close bracket context");
      }
    }

    private IEnumerable<(MathExpressionTokenType Type, string Value)> ProccessFunctionContext((MathExpressionTokenType Type, string Value) token)
    {
      if (token.Type != MathExpressionTokenType.OpenBracket)
      {
        throw new ArgumentException($"Unexpected token type '{token.Type}' in function context");
      }

      context = MathExpressionTokenType.Unknown;
      yield return token;
    }

    private bool HasNumber() => currentNumber.Count > 0;

    private (MathExpressionTokenType, string) PopNumber()
    {
      var result = Join(currentNumber.Reverse());
      currentNumber.Clear();
      return (MathExpressionTokenType.Number, result);
    }

    private void PushNumber(string digit) => currentNumber.Push(digit);

    private bool TryGetSign(out string sign)
    {
      if (currentNumber.Any(n => n != "-"))
      {
        currentNumber.Clear();
        sign = null;
        return false;
      }

      sign = (currentNumber.Count() & 1) > 0 ? "-" : "+";
      currentNumber.Clear();
      return true;
    }

    private static string Join(IEnumerable<string> strings) => string.Join(string.Empty, strings);

    private static bool IsExponent(string token)
      => string.Equals(token, Exponent, StringComparison.OrdinalIgnoreCase);

    private static bool IsSign(string token) => token == "-" || token == "+";
  }
}