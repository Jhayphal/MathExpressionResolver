using System;
using System.Linq.Expressions;

namespace MathExpressionResolver
{
  internal class Program
  {
    static void Main()
    {
      var expression = "((2 + 3) * (1 + 2)) * 4 & -2";
      CalculateAndPrint(expression);
    }

    static void CalculateAndPrint(string expression)
    {
      var supportedOperations = SupportedOperations.GetSupported();
      var tokenizer = new MathExpressionTokenizer(supportedOperations);

      var tokens = tokenizer.GetTokens(expression);
      var reversePolishNotation = ShuntingYard.Convert(tokens, tokenizer.Operations);
      var result = ReversePolishNotationResolver.Calculate(reversePolishNotation, tokenizer.Operations);

      Console.WriteLine($"{expression} = {result}");

      Console.ReadLine();
    }

    static void CalculateAndPrintDetailed(string expression)
    {
      var supportedOperations = SupportedOperations.GetSupported();
      var tokenizer = new MathExpressionTokenizer(supportedOperations);

      Console.WriteLine("Expression: " + expression);

      var tokens = tokenizer.GetTokens(expression);

      Console.WriteLine();
      Console.WriteLine("Tokens: ");

      foreach (var item in tokens)
        Console.WriteLine(item.Type.ToString() + ": " + item.Value);

      var reversePolishNotation = ShuntingYard.Convert(tokens, tokenizer.Operations);

      Console.WriteLine();
      Console.WriteLine("Reverse Polish notation: ");

      foreach (var item in reversePolishNotation)
        Console.WriteLine(item.Type.ToString() + ": " + item.Value);

      var result = ReversePolishNotationResolver.Calculate(reversePolishNotation, tokenizer.Operations);

      Console.WriteLine();
      Console.WriteLine("Result: " + result);

      Console.ReadLine();
    }
  }
}