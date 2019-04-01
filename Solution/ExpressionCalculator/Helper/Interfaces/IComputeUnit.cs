using System;

namespace Solution.ExpressionCalculator
{
  public interface IComputeUnit: IExpression
  {
    Double Compute();
  }
}
