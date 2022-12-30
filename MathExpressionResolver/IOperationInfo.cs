using System;

namespace MathExpressionResolver
{
  public interface IOperationInfo : IEquatable<IOperationInfo>, IComparable<IOperationInfo>
  {
    string Operator { get; }

    int Priority { get; }

    double Calculate(params double[] args);
  }
}