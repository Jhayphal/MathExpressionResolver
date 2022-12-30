using System;

namespace MathExpressionResolver
{
  public class BinaryFunctionInfo : OperationInfoBase
  {
    private readonly Func<double, double, double> calculate;

    public BinaryFunctionInfo(string @operator, int priority, Func<double, double, double> calculate)
      : base(@operator, priority)
    {
      this.calculate = calculate ?? throw new ArgumentNullException(nameof(calculate));
    }

    public override double Calculate(params double[] args) => calculate(args[0], args[1]);
  }
}