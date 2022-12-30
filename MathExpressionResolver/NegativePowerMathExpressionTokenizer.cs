using System.Collections.Generic;

namespace MathExpressionResolver
{
  internal sealed class NegativePowerMathExpressionTokenizer : MathExpressionTokenizer
  {
    public NegativePowerMathExpressionTokenizer(SupportedOperations supportedOperations)
      : base(supportedOperations)
    {
    }

    public override IEnumerable<(MathExpressionTokenType Type, string Value)> GetTokens(string expression)
    {
      (MathExpressionTokenType Type, string Value) previously = (MathExpressionTokenType.Unknown, string.Empty);

      foreach (var current in base.GetTokens(expression))
      {
        if (previously.Type == MathExpressionTokenType.Number && previously.Value.StartsWith("-")
          && current.Type == MathExpressionTokenType.Operator && current.Value == "&")
        {
          yield return (MathExpressionTokenType.Number, "-1");
          yield return (MathExpressionTokenType.Operator, "*");
          yield return (MathExpressionTokenType.OpenBracket, "(");
          yield return (MathExpressionTokenType.Number, previously.Value.Substring(1));
          yield return (MathExpressionTokenType.CloseBracket, ")");
        }
        else if (previously.Type != MathExpressionTokenType.Unknown)
        {
          yield return previously;
        }

        previously = current;
      }

      if (previously.Type != MathExpressionTokenType.Unknown)
      {
        yield return previously;
      }
    }
  }
}