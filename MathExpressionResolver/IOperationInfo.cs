using System;

namespace MathExpressionResolver
{
  internal partial class SupportedOperations
  {
    public interface IOperationInfo : IEquatable<IOperationInfo>, IComparable<IOperationInfo>
    {
      string Operator { get; }

      int Priority { get; }

      double Calculate(params double[] args);
    }
  }
}