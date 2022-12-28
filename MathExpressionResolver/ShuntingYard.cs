using System;
using System.Collections.Generic;

namespace MathExpressionResolver
{
  internal static class ShuntingYard
  {
    public static Queue<(MathExpressionTokenType Type, string Value)> Convert(IEnumerable<(MathExpressionTokenType Type, string Operator)> tokens, SupportedOperators supportedOperators)
    {
      Queue<(MathExpressionTokenType Type, string Value)> outputQueue = new Queue<(MathExpressionTokenType Type, string Value)>();
      Stack<(MathExpressionTokenType Type, string Value)> stack = new Stack<(MathExpressionTokenType Type, string Value)>();

      foreach (var token in tokens)
      {
        switch (token.Type)
        {
          case MathExpressionTokenType.Number:
            // Если токен — число, то добавить его в очередь вывода
            outputQueue.Enqueue(token);

            break;

          case MathExpressionTokenType.Operator:
            /* Пока присутствует на вершине стека токен оператор op2, 
             * чей приоритет выше или равен приоритету op1, и 
             * при равенстве приоритетов op1 является левоассоциативным:
             * Переложить op2 из стека в выходную очередь*/
            while (NeedNextAction(token, outputQueue, stack, supportedOperators)) ;

            // Положить op1 в стек
            stack.Push(token);

            break;

          case MathExpressionTokenType.OpenBracket:
            // Если токен — открывающая скобка, то положить его в стек
            stack.Push(token);

            break;

          case MathExpressionTokenType.CloseBracket:
            // Пока токен на вершине стека не открывающая скобка
            while (stack.Count > 0 && stack.Peek().Type != MathExpressionTokenType.OpenBracket)
            {
              // Переложить оператор из стека в выходную очередь
              outputQueue.Enqueue(stack.Pop());

              // Если стек закончился до того, как был встречен токен открывающая скобка, то в выражении пропущена скобка
              if (stack.Count == 0)
                throw new ArithmeticException("В выражении пропущена скобка.");
            }

            // Выкинуть открывающую скобку из стека, но не добавлять в очередь вывода
            stack.Pop();

            break;

          default:
            throw new NotImplementedException();
        }
      }

      // Если больше не осталось токенов на входе
      // Пока есть токены операторы в стеке
      while (stack.Count > 0)
      {
        var token = stack.Pop();

        // Если токен оператор на вершине стека — открывающая скобка, то в выражении пропущена скобка
        if (token.Type == MathExpressionTokenType.OpenBracket)
          throw new ArithmeticException("В выражении пропущена скобка.");

        // Переложить оператор из стека в выходную очередь
        outputQueue.Enqueue(token);
      }

      return outputQueue;
    }

    private static bool NeedNextAction
    (
        (MathExpressionTokenType Type, string Value) current,
        Queue<(MathExpressionTokenType Type, string Value)> outputQueue,
        Stack<(MathExpressionTokenType Type, string Value)> stack,
        SupportedOperators supportedOperators
    )
    {
      if (stack.Count == 0)
        return false;

      var previously = stack.Peek();

      if (previously.Type == MathExpressionTokenType.Operator)
      {
        var compare = supportedOperators.CompareTo(previously.Value, current.Value);

        if (compare > 0
            || compare == 0 && supportedOperators.IsLeftAssociative(previously.Value))
        {
          outputQueue.Enqueue(stack.Pop());

          return true;
        }
      }

      return false;
    }
  }
}