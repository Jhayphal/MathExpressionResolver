using System;
using System.Collections;
using System.Collections.Generic;

namespace MathExpressionResolver
{
  internal partial class SupportedOperations : IEnumerable<IOperationInfo>
  {
    private readonly HashSet<IOperationInfo> Operators = new HashSet<IOperationInfo>();

    public static SupportedOperations GetSupported()
    {
      var result = new SupportedOperations();

      result.Add(@operator: "log", priority: 0, calculate: x => Math.Log(x));
      result.Add(@operator: "ln", priority: 0, calculate: Math.Log10);
      result.Add(@operator: "exp", priority: 0, calculate: Math.Exp);
      result.Add(@operator: "sqrt", priority: 0, calculate: Math.Sqrt);
      result.Add(@operator: "abs", priority: 0, calculate: Math.Abs);
      result.Add(@operator: "atan", priority: 0, calculate: Math.Atan);
      result.Add(@operator: "acos", priority: 0, calculate: Math.Acos);
      result.Add(@operator: "asin", priority: 0, calculate: Math.Asin);
      result.Add(@operator: "sinh", priority: 0, calculate: Math.Sinh);
      result.Add(@operator: "cosh", priority: 0, calculate: Math.Cosh);
      result.Add(@operator: "tanh", priority: 0, calculate: Math.Tanh);
      result.Add(@operator: "tan", priority: 0, calculate: Math.Tan);
      result.Add(@operator: "sin", priority: 0, calculate: Math.Sin);
      result.Add(@operator: "cos", priority: 0, calculate: Math.Cos);
      result.Add(@operator: "cos", priority: 0, calculate: Math.Cos);
      result.Add(@operator: "+", priority: 1, leftAssociative: true, calculate: (a, b) => a + b);
      result.Add(@operator: "-", priority: 1, leftAssociative: true, calculate: (a, b) => a - b);
      result.Add(@operator: "*", priority: 2, leftAssociative: true, calculate: (a, b) => a * b);
      result.Add(@operator: "/", priority: 2, leftAssociative: true,
        calculate: (a, b) => b == 0 ? throw new DivideByZeroException() : a / b);
      result.Add(@operator: "&", priority: 3, leftAssociative: false, calculate: Math.Pow);

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
      {
        if (item.Equals(@operator))
        {
          return true;
        }
      }

      return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private IOperationInfo GetOperator(string @operator)
    {
      foreach (var item in Operators)
      {
        if (item.Equals(@operator))
        {
          return item;
        }
      }

      throw new ArgumentException($"Оператора '{@operator}' не существует.");
    }
  }
}