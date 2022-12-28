using System;
using System.Linq;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;

namespace MathExpressionResolver
{
  internal interface ITokenizer<TTokenType>
  {
    IEnumerable<(TTokenType Type, string Value)> GetTokens(string expression);
  }

  internal sealed class MathExpressionTokenizer : ITokenizer<MathExpressionTokenType>
  {
    private readonly Tokenizer<MathExpressionTokenType> tokenizer;

    public MathExpressionTokenizer(SupportedOperators supportedOperators)
    {
      Operators = supportedOperators;

      var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
      var tokens = new Dictionary<MathExpressionTokenType, HashSet<string>>
      {
        { MathExpressionTokenType.Operator, 
          new HashSet<string>(supportedOperators.Select(o => o.Operator)) },
        { MathExpressionTokenType.OpenBracket, 
          new HashSet<string> { "(" } },
        { MathExpressionTokenType.CloseBracket, 
          new HashSet<string> { ")" } },
        { MathExpressionTokenType.Number, 
          new HashSet<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", decimalSeparator } }
      };

      tokenizer = new Tokenizer<MathExpressionTokenType>(tokens, caseSensetive: false);
    }

    public readonly SupportedOperators Operators;

    public IEnumerable<(MathExpressionTokenType Type, string Value)> GetTokens(string expression)
    {
      var currentNumber = new List<string>();

      foreach (var (tokenType, token) in tokenizer.GetTokens(expression))
      {
        if (tokenType == MathExpressionTokenType.Number)
        {
          currentNumber.Add(token);
        }
        else
        {
          if (currentNumber.Count > 0)
          {
            yield return (MathExpressionTokenType.Number, Join(currentNumber));
            currentNumber.Clear();
          }

          yield return (tokenType, token);
        }
      }

      if (currentNumber.Count > 0)
        yield return (MathExpressionTokenType.Number, Join(currentNumber));
    }

    private string Join(IEnumerable<string> strings) => string.Join(string.Empty, strings);
  }

  internal sealed class Tokenizer<TTokenType> : ITokenizer<TTokenType>
  {
    private readonly Parser parser;
    private readonly IDictionary<TTokenType, HashSet<string>> tokens;

    public Tokenizer(IDictionary<TTokenType, HashSet<string>> tokens, bool caseSensetive)
    {
      this.tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
      parser = new Parser(this.tokens.SelectMany(t => t.Value), caseSensetive);
    }

    public IEnumerable<(TTokenType Type, string Value)> GetTokens(string expression)
    {
      foreach (var token in parser.Parse(expression))
      {
        bool found = false;

        foreach (var pair in tokens)
        {
          if (found = pair.Value.Contains(token))
          {
            yield return (pair.Key, token);

            break;
          }
        }

        if (!found)
          throw new InvalidCastException($"Undefined token: {token}");
      }
    }
  }
}