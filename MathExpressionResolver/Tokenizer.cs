using System;
using System.Collections.Generic;
using System.Text;

namespace MathExpressionResolver
{
    internal class Tokenizer
    {
        public readonly SupportedOperators Operators;

        private readonly string NumberDecimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        public Tokenizer(SupportedOperators supportedOperators)
        {
            Operators = supportedOperators 
                ?? throw new ArgumentNullException(nameof(supportedOperators));
        }

        public IEnumerable<(TokenType Type, string Value)> GetTokens(string expression)
        {
            int index = 0;

            while (index < expression.Length)
            {
                bool openBracket;
                int newIndex;
                bool negative;

                while (TryReadBracket(expression, index, out openBracket, out newIndex, out negative)
                    && openBracket) // после оператора или перед операндом не может быть закрывающей скобки
                {
                    if (index == newIndex)
                        throw new InvalidOperationException();

                    index = newIndex;

                    foreach (var item in GetBrackets(openBracket, negative))
                        yield return item;
                }

                if (index == expression.Length)
                    break;

                yield return (TokenType.Number, ReadNumber(expression, index, out newIndex));

                if (index == newIndex)
                    throw new InvalidOperationException();

                index = newIndex;

                if (index == expression.Length)
                    break;

                while (TryReadBracket(expression, index, out openBracket, out newIndex, out negative) 
                    && !openBracket) // после операнда не может быть открывающей скобки
                {
                    if (index == newIndex)
                        throw new InvalidOperationException();

                    index = newIndex;

                    foreach (var item in GetBrackets(openBracket, negative))
                        yield return item;
                }

                if (index == expression.Length)
                    break;

                yield return (TokenType.Operator, ReadOperator(expression, index, out newIndex));
                
                if (index == newIndex)
                    throw new InvalidOperationException();

                index = newIndex;

                if (index == expression.Length)
                    break;
            }
        }

        private IEnumerable<(TokenType Type, string Value)> GetBrackets(bool openBracket, bool negative)
        {
            if (negative)
            {
                yield return (TokenType.Number, "-1");
                yield return (TokenType.Operator, "*");
            }

            if (openBracket)
                yield return (TokenType.OpenBracket, "(");
            else
                yield return (TokenType.CloseBracket, ")");
        }

        private bool TryReadBracket(
            string expression, 
            int index, 
            out bool openBracket, 
            out int newIndex, 
            out bool negative)
        {
            if (index == expression.Length)
            {
                openBracket = false;
                negative = false;
                newIndex = -1;

                return false;
            }

            while (char.IsWhiteSpace(expression[index]))
                ++index;

            var current = expression[index];

            if (current == '(')
            {
                openBracket = true;
                negative = false;

                newIndex = index + 1;

                return true;
            }
            else if (current == ')')
            {
                openBracket = false;
                negative = false;

                newIndex = index + 1;

                return true;
            }
            else if (current == '-' || current == '+')
            {
                int bracketIndex = index + 1;

                while (char.IsWhiteSpace(expression[bracketIndex]))
                    ++bracketIndex;

                current = expression[bracketIndex];

                if (current == '(')
                {
                    openBracket = true;
                    negative = expression[index] == '-';

                    newIndex = bracketIndex + 1;

                    return true;
                }
                else if (current == ')')
                {
                    openBracket = false;
                    negative = expression[index] == '-';

                    newIndex = bracketIndex + 1;

                    return true;
                }
            }

            openBracket = false;
            negative = false;
            newIndex = -1;

            return false;
        }

        private string ReadOperator(string expression, int startIndex, out int newIndex)
        {
            while (char.IsWhiteSpace(expression[startIndex]))
                ++startIndex;

            var current = expression[startIndex];

            if (Operators.IsSupported(current))
            {
                newIndex = startIndex + 1;

                return current.ToString();
            }

            throw new NotSupportedException(nameof(current));
        }

        private string ReadNumber(string expression, int startIndex, out int newIndex)
        {
            while (char.IsWhiteSpace(expression[startIndex]))
                ++startIndex;

            StringBuilder result = new StringBuilder();

            var current = expression[startIndex];

            if (current == '-' || current == '+')
            {
                result.Append(current);

                ++startIndex;
            }

            while (startIndex < expression.Length)
            {
                current = expression[startIndex];

                if (char.IsWhiteSpace(current))
                {
                    ++startIndex;

                    continue;
                }

                if (char.IsDigit(current))
                {
                    result.Append(current);

                    ++startIndex;

                    continue;
                }

                if (current == '.'
                    || current == ',')
                {
                    result.Append(NumberDecimalSeparator);

                    ++startIndex;

                    continue;
                }

                break;
            }

            newIndex = startIndex;

            return result.ToString();
        }
    }
}
