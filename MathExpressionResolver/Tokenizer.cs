using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathExpressionResolver
{
  internal sealed class TokenizerEx
  {
    private readonly Parser parser;
    private readonly Dictionary<TokenType, HashSet<string>> tokens = new Dictionary<TokenType, HashSet<string>>
    {
      { TokenType.OpenBracket, new HashSet<string> { "(" } },
      { TokenType.CloseBracket, new HashSet<string> { ")" } },
      { TokenType.Number, new HashSet<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" } }
    };

    public TokenizerEx(SupportedOperators supportedOperators)
    {
      Operators = supportedOperators ?? throw new ArgumentNullException(nameof(supportedOperators));
      tokens[TokenType.Operator] = new HashSet<string>(Operators.Select(o => o.Operator));
      tokens[TokenType.Number].Add(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
      parser = new Parser(tokens: tokens.SelectMany(t => t.Value), caseSensetive: false);
    }

    public readonly SupportedOperators Operators;

    public IEnumerable<(TokenType Type, string Value)> GetTokens(string expression)
    {
      var currentNumber = new List<string>();

      foreach (var token in parser.Parse(expression))
        foreach (var pair in tokens)
        {
          if (pair.Value.Contains(token))
            if (pair.Key == TokenType.Number)
            {
              currentNumber.Add(token);
            }
            else
            {
              if (currentNumber.Count > 0)
              {
                yield return (TokenType.Number, string.Join(string.Empty, currentNumber));
                currentNumber.Clear();
              }

              yield return (pair.Key, token);
            }
        }

      if (currentNumber.Count > 0)
        yield return (TokenType.Number, string.Join(string.Empty, currentNumber));
    }
  }
}