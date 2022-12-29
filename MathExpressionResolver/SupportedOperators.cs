using System;
using System.Collections;
using System.Collections.Generic;
using static MathExpressionResolver.SupportedOperations;

namespace MathExpressionResolver
{
  internal partial class SupportedOperations : IEnumerable<IOperationInfo>
  {
    private readonly HashSet<IOperationInfo> Operators = new HashSet<IOperationInfo>();

    public static SupportedOperations GetSupported()
    {
      var result = new SupportedOperations();

      result.Add(@operator: "+", priority: 0, leftAssociative: true, calculate: (a, b) => a + b);
      result.Add(@operator: "-", priority: 0, leftAssociative: true, calculate: (a, b) => a - b);
      result.Add(@operator: "*", priority: 1, leftAssociative: true, calculate: (a, b) => a * b);
      result.Add(@operator: "/", priority: 1, leftAssociative: true, calculate: (a, b) => a / b);
      result.Add(@operator: "&", priority: 2, leftAssociative: false, calculate: (a, b) => Math.Pow(a, b));
      result.Add(@operator: "abs", priority: 3, calculate: a => Math.Abs(a));

      return result;
    }

    public bool Add(string @operator, int priority, bool leftAssociative, Func<double, double, double> calculate)
    {
      var newItem = new OperatorInfo(@operator, priority, leftAssociative, calculate);

      return Operators.Add(newItem);
    }

    public bool Add(string @operator, int priority, Func<double, double> calculate)
    {
      var newItem = new UnaryFunctionInfo(@operator, priority, calculate);

      return Operators.Add(newItem);
    }

    public bool Add(string @operator, int priority, Func<double, double, double> calculate)
    {
      var newItem = new BinaryFunctionInfo(@operator, priority, calculate);

      return Operators.Add(newItem);
    }

    public double Calculate(string @operator, params double[] args) => GetOperator(@operator).Calculate(args);

    public int CompareTo(string operatorToCompare, string compareWith)
      => GetOperator(operatorToCompare).CompareTo(GetOperator(compareWith));

    public IEnumerator<IOperationInfo> GetEnumerator() => Operators.GetEnumerator();

    public int GetOperatorPriority(char @operator) => GetOperatorPriority(@operator.ToString());

    public int GetOperatorPriority(string @operator) => GetOperator(@operator).Priority;

    public bool IsLeftAssociative(char @operator) => IsLeftAssociative(@operator.ToString());

    public bool IsLeftAssociative(string @operator) => GetOperator(@operator) is OperatorInfo currentOperator && currentOperator.LeftAssociative;

    public bool IsSupported(char @operator) => IsSupported(@operator.ToString());

    public bool IsSupported(string @operator)
    {
      foreach (var item in Operators)
        if (item.Equals(@operator))
          return true;

      return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private IOperationInfo GetOperator(string @operator)
    {
      foreach (var item in Operators)
        if (item.Equals(@operator))
          return item;

      throw new ArgumentException($"Оператора '{@operator}' не существует.");
    }
  }
}