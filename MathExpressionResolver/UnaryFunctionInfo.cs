using System;

namespace MathExpressionResolver
{
  public class UnaryFunctionInfo : OperationInfoBase
  {
    private readonly Func<double, double> calculate;

    public UnaryFunctionInfo(string @operator, int priority, Func<double, double> calculate)
      : base(@operator, priority)
    {
      this.calculate = calculate ?? throw new ArgumentNullException(nameof(calculate));
    }

    public override double Calculate(params double[] args) => calculate(args[0]);
  }
}