using System;

namespace MathExpressionResolver
{
  internal partial class SupportedOperators
  {
    public interface IOperatorInfo : IEquatable<OperatorInfo>, IComparable<OperatorInfo>
    {
      string Operator { get; }

      int Priority { get; }
    }

    public class OperatorInfo : IOperatorInfo
    {
      public string Operator { get; }

      public int Priority { get; }

      public readonly bool LeftAssociative;

      public readonly Func<double, double, double> Calculate;

      public OperatorInfo(string @operator, int priority, bool leftAssociative, Func<double, double, double> calculate)
      {
        if (string.IsNullOrWhiteSpace(@operator))
          throw new ArgumentException(nameof(@operator));

        Operator = @operator;
        Priority = priority;
        LeftAssociative = leftAssociative;

        Calculate = calculate ?? throw new ArgumentNullException(nameof(calculate));
      }

      public int CompareTo(OperatorInfo other)
      {
        if (other == null)
          throw new ArgumentNullException(nameof(other));

        return Priority.CompareTo(other.Priority);
      }

      public override bool Equals(object obj)
      {
        if (obj == null)
          return false;

        if (obj is OperatorInfo @operator)
          return Equals(@operator.Operator);

        if (obj is string @operatorString)
          return Equals(@operatorString);

        if (obj is char @operatorChar)
          return Equals(@operatorChar);

        return Equals(obj.ToString());
      }

      public bool Equals(OperatorInfo other) => Equals(other?.Operator);

      public bool Equals(string other) => string.Equals(Operator, other);

      public bool Equals(char other) => Equals(other.ToString());

      public override int GetHashCode() => Operator.GetHashCode();

      public override string ToString() => Operator;
    }
  }
}