using System;
using System.Collections.Generic;

namespace MathExpressionResolver
{
  internal class Program
  {
    private static readonly List<(string, double)> samples = new List<(string, double)>
    {
      ("--- 5* 2", -10d),
      ("2 * 3 + 2 & 2 * 4", 22d),
      ("5 -- 6", 11d),
      ("((2 + 3) * (1 + 2)) * 4 & 2 ", 240d),
      ("((2 + 3) * (1 + 2)) * 4 & -2", 0.9375),
      ("sqrt (sin(2 + 3)*cos (1+2)) * 4 & 2", 15.5893529757165),
      ("sqrt (-2)", double.NegativeInfinity),
      ("1 / 0", double.NegativeInfinity),
      ("Abs(-2 * 1e-3)", 0.002),
      ("4 & 3 & 2", 262144d),
      ("sqrt(-5&(7+1--1+-7))", double.NegativeInfinity),
      ("-5&3&2*2-1", -3906251d),
      ("-2&2", -4d),
      ("sqrt(-5&(-1+1--1+--1))", double.NegativeInfinity),
      ("(Abs(1+(2--1)-3)* sin(3 + -1) / 2.1) -12.3453* 0.45+2.4e3", 2394.877613774679)
    };

    static void Main()
    {
      CheckSamples();

      Console.ReadLine();
    }

    static void CheckSamples()
    {
      var supportedOperations = SupportedOperations.GetSupported();
      var tokenizer = new NegativePowerMathExpressionTokenizer(supportedOperations);

      foreach (var sample in samples)
      {
        CalculateAndCheck(tokenizer, sample.Item1, sample.Item2);
      }
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

    static void CalculateAndCheck(MathExpressionTokenizer tokenizer, string expression, double expected)
    {
      var tokens = tokenizer.GetTokens(expression);
      var reversePolishNotation = ShuntingYard.Convert(tokens, tokenizer.Operations);
      
      double result;
      try
      {
        result = ReversePolishNotationResolver.Calculate(reversePolishNotation, tokenizer.Operations);
      }
      catch
      {
        result = double.NegativeInfinity;
      }

      if (Math.Abs(result - expected) > 0.000001)
      {
        Console.WriteLine($"{expression} = {result}");
      }
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
      {
        Console.WriteLine(item.Type.ToString() + ": " + item.Value);
      }

      var reversePolishNotation = ShuntingYard.Convert(tokens, tokenizer.Operations);

      Console.WriteLine();
      Console.WriteLine("Reverse Polish notation: ");

      foreach (var item in reversePolishNotation)
      {
        Console.WriteLine(item.Type.ToString() + ": " + item.Value);
      }

      var result = ReversePolishNotationResolver.Calculate(reversePolishNotation, tokenizer.Operations);

      Console.WriteLine();
      Console.WriteLine("Result: " + result);

      Console.ReadLine();
    }
  }
}