using System;
using System.Collections.Generic;
using System.Linq;

namespace MathExpressionResolver
{
  internal sealed class Parser
  {
    private readonly HashSet<string> tokens;
    private readonly bool caseSensetive;

    public Parser(IEnumerable<string> tokens, bool caseSensetive)
    {
      if (!(tokens?.Any()).GetValueOrDefault())
        throw new ArgumentException(nameof(tokens));

      this.caseSensetive = caseSensetive;
      this.tokens = new HashSet<string>(this.caseSensetive ? tokens : tokens.Select(s => s.ToLowerInvariant())); 
    }

    public IEnumerable<string> Parse(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        yield break;

      Queue<char> currentToken = new Queue<char>();
      foreach (var c in text)
      {
        if (char.IsWhiteSpace(c))
        {
          if (currentToken.Count > 0)
            throw new ArgumentException(new string(currentToken.ToArray()));

          continue;
        }

        currentToken.Enqueue(c);

        if (IsValidToken(currentToken))
        {
          yield return new string(currentToken.ToArray());
          
          currentToken.Clear();
        }
      }

      if (currentToken.Count > 0)
        throw new ArgumentException(currentToken.ToString());
    }

    private bool IsValidToken(Queue<char> currentToken)
    {
      // можно не тратить память и ускорить поиском в отсортированном списке
      var target = new string(currentToken.ToArray());

      if (!caseSensetive)
        target = target.ToLowerInvariant();

      return tokens.Contains(target);
    }
  }
}