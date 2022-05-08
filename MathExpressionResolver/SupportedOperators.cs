using System;
using System.Collections.Generic;

namespace MathExpressionResolver
{
    internal partial class SupportedOperators
    {
        private readonly HashSet<OperatorInfo> Operators = new HashSet<OperatorInfo>();

        public static SupportedOperators GetSupported()
        {
            var result = new SupportedOperators();

            result.Add(@operator: "+", priority: 0, leftAssociative: true, calculate: (a, b) => a + b);
            result.Add(@operator: "-", priority: 0, leftAssociative: true, calculate: (a, b) => a - b);
            result.Add(@operator: "*", priority: 1, leftAssociative: true, calculate: (a, b) => a * b);
            result.Add(@operator: "/", priority: 1, leftAssociative: true, calculate: (a, b) => a / b);
            result.Add(@operator: "^", priority: 2, leftAssociative: false, calculate: (a, b) => Math.Pow(a, b));

            return result;
        }

        public bool Add(string @operator, int priority, bool leftAssociative, Func<double, double, double> calculate)
        {
            var newItem = new OperatorInfo(@operator, priority, leftAssociative, calculate);

            return Operators.Add(newItem);
        }

        public double Calculate(string @operator, double a, double b)
        {
            var info = GetOperator(@operator);

            return info.Calculate(a, b);
        }

        public int CompareTo(string operatorToCompare, string compareWith)
        {
            return GetOperator(operatorToCompare).CompareTo(GetOperator(compareWith));
        }

        public int GetOperatorPriority(char @operator)
        {
            return GetOperatorPriority(@operator.ToString());
        }

        public int GetOperatorPriority(string @operator)
        {
            return GetOperator(@operator).Priority;
        }

        public bool IsLeftAssociative(char @operator)
        {
            return IsLeftAssociative(@operator.ToString());
        }

        public bool IsLeftAssociative(string @operator)
        {
            return GetOperator(@operator).LeftAssociative;
        }

        public bool IsSupported(char @operator)
        {
            return IsSupported(@operator.ToString());
        }

        public bool IsSupported(string @operator)
        {
            foreach (var item in Operators)
                if (item.Equals(@operator))
                    return true;

            return false;
        }

        private OperatorInfo GetOperator(string @operator)
        {
            foreach (var item in Operators)
                if (item.Equals(@operator))
                    return item;

            throw new ArgumentException($"Оператора '{@operator}' не существует.");
        }
    }
}
