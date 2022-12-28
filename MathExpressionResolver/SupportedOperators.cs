using System;
using System.Collections;
using System.Collections.Generic;
using static MathExpressionResolver.SupportedOperators;

namespace MathExpressionResolver
{
  internal partial class SupportedOperators : IEnumerable<OperatorInfo>
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
      result.Add(@operator: "abs", priority: 3, leftAssociative: false, calculate: (a, b) => Math.Pow(a, b));

      return result;
    }

    public bool Add(string @operator, int priority, bool leftAssociative, Func<double, double, double> calculate)
    {
      var newItem = new OperatorInfo(@operator, priority, leftAssociative, calculate);

      return Operators.Add(newItem);
    }

    public double Calculate(string @operator, double a, double b) => GetOperator(@operator).Calculate(a, b);

    public int CompareTo(string operatorToCompare, string compareWith)
      => GetOperator(operatorToCompare).CompareTo(GetOperator(compareWith));

    public IEnumerator<OperatorInfo> GetEnumerator() => Operators.GetEnumerator();

    public int GetOperatorPriority(char @operator) => GetOperatorPriority(@operator.ToString());

    public int GetOperatorPriority(string @operator) => GetOperator(@operator).Priority;

    public bool IsLeftAssociative(char @operator) => IsLeftAssociative(@operator.ToString());

    public bool IsLeftAssociative(string @operator) => GetOperator(@operator).LeftAssociative;

    public bool IsSupported(char @operator) => IsSupported(@operator.ToString());

    public bool IsSupported(string @operator)
    {
      foreach (var item in Operators)
        if (item.Equals(@operator))
          return true;

      return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private OperatorInfo GetOperator(string @operator)
    {
      foreach (var item in Operators)
        if (item.Equals(@operator))
          return item;

      throw new ArgumentException($"Оператора '{@operator}' не существует.");
    }
  }
}