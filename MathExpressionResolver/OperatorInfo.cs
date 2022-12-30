using System;

namespace MathExpressionResolver
{
  public class OperatorInfo : OperationInfoBase, IOperator
  {
    private readonly Func<double, double, double> calculate;

    public OperatorInfo(string @operator, int priority, bool leftAssociative, Func<double, double, double> calculate)
      : base(@operator, priority)
    {
      LeftAssociative = leftAssociative;

      this.calculate = calculate ?? throw new ArgumentNullException(nameof(calculate));
    }

    public bool LeftAssociative { get; }

    public override double Calculate(params double[] args) => calculate(args[0], args[1]);
  }
}