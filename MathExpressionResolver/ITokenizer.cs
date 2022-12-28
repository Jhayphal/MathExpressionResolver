using System.Collections.Generic;

namespace MathExpressionResolver
{
  internal interface ITokenizer<TTokenType>
  {
    IEnumerable<(TTokenType Type, string Value)> GetTokens(string expression);
  }
}