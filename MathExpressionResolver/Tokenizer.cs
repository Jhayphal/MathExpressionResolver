using System;
using System.Linq;
using System.Collections.Generic;

namespace MathExpressionResolver
{
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
        {
          throw new InvalidCastException($"Undefined token: {token}");
        }
      }
    }
  }
}