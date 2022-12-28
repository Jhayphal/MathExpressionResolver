using System;
using System.Diagnostics;

namespace MathExpressionResolver
{
  internal partial class SupportedOperations
  {
    [DebuggerDisplay("{Priority}x'{Operator}'")]
    public abstract class OperationInfoBase : IOperationInfo
    {
      public OperationInfoBase(string @operator, int priority)
      {
        if (string.IsNullOrWhiteSpace(@operator))
          throw new ArgumentException(nameof(@operator));

        Operator = @operator;
        Priority = priority;
      }

      public string Operator { get; }

      public int Priority { get; }

      public abstract double Calculate(params double[] args);

      public int CompareTo(IOperationInfo other)
      {
        if (other == null)
          throw new ArgumentNullException(nameof(other));

        return Priority.CompareTo(other.Priority);
      }

      public override bool Equals(object obj)
      {
        if (obj == null)
          return false;

        if (obj is IOperationInfo @operator)
          return Equals(@operator.Operator);

        if (obj is string @operatorString)
          return Equals(@operatorString);

        if (obj is char @operatorChar)
          return Equals(@operatorChar);

        return Equals(obj.ToString());
      }

      public bool Equals(IOperationInfo other) => Equals(other?.Operator);

      public bool Equals(string other) => string.Equals(Operator, other);

      public bool Equals(char other) => Equals(other.ToString());

      public override int GetHashCode() => Operator.GetHashCode();

      public override string ToString() => Operator;
    }
  }
}