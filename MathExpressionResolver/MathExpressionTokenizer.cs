using System.Linq;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;

namespace MathExpressionResolver
{
  internal sealed class MathExpressionTokenizer : ITokenizer<MathExpressionTokenType>
  {
    private readonly Tokenizer<MathExpressionTokenType> tokenizer;

    public MathExpressionTokenizer(SupportedOperations supportedOperations)
    {
      Operations = supportedOperations;

      var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
      var tokens = new Dictionary<MathExpressionTokenType, HashSet<string>>
      {
        { MathExpressionTokenType.Operator, 
          new HashSet<string>(supportedOperations.Select(o => o.Operator)) },
        { MathExpressionTokenType.OpenBracket, 
          new HashSet<string> { "(" } },
        { MathExpressionTokenType.CloseBracket, 
          new HashSet<string> { ")" } },
        { MathExpressionTokenType.Number, 
          new HashSet<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", decimalSeparator } }
      };

      tokenizer = new Tokenizer<MathExpressionTokenType>(tokens, caseSensetive: false);
    }

    public readonly SupportedOperations Operations;

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
}