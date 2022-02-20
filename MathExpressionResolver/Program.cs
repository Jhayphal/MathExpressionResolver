using System;

namespace MathExpressionResolver
{
    internal class Program
    {
        static void Main()
        {
            var supportedOperators = SupportedOperators.GetSupported();
            var tokenizer = new Tokenizer(supportedOperators);

            var expression = "((3 - 1) * 5.5 - 2^2) + 3";

            Console.WriteLine("Expression: " + expression);

            var tokens = tokenizer.GetTokens(expression);

            Console.WriteLine();
            Console.WriteLine("Tokens: ");

            foreach (var item in tokens)
                Console.WriteLine(item.Type.ToString() + ": " + item.Value);

            var reversePolishNotation = ShuntingYard.Convert(tokens, tokenizer.Operators);

            Console.WriteLine();
            Console.WriteLine("Reverse Polish notation: ");

            foreach (var item in reversePolishNotation)
                Console.WriteLine(item.Type.ToString() + ": " + item.Value);

            var result = ReversePolishNotationResolver.Calculate(reversePolishNotation, tokenizer.Operators);

            Console.WriteLine();
            Console.WriteLine("Result: " + result);

            Console.ReadLine();
        }
    }
}
