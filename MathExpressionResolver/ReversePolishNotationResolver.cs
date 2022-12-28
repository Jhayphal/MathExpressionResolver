using System;
using System.Collections.Generic;

namespace MathExpressionResolver
{
  internal static class ReversePolishNotationResolver
  {
    public static double Calculate(Queue<(MathExpressionTokenType Type, string Value)> queue, SupportedOperators operators)
    {
      if (queue == null)
        throw new ArgumentNullException(nameof(queue));

      Stack<double> operands = new Stack<double>();

      if (queue.Count == 0)
        return 0d;

      while (queue.Count > 0)
      {
        var current = queue.Dequeue();

        switch (current.Type)
        {
          case MathExpressionTokenType.Number:
            var value = double.Parse(current.Value);

            operands.Push(value);

            break;

          case MathExpressionTokenType.Operator:
            var b = operands.Pop();
            var a = operands.Pop();

            var operationResult = operators.Calculate(current.Value, a, b);

            operands.Push(operationResult);

            break;

          default:
            throw new NotImplementedException(current.Type.ToString());
        }
      }

      return operands.Pop();
    }
  }
}
