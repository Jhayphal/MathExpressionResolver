namespace MathExpressionResolver
{
  internal partial class SupportedOperations
  {
    public interface IOperator
    {
      bool LeftAssociative { get; }
    }
  }
}