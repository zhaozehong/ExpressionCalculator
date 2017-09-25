using System;

namespace Solution.ExpressionCalculator
{
  public class NumericUnit : ComputeUnit
  {
    public NumericUnit(String strExpression) : base(strExpression) { }

    public override Double Compute()
    {
      try
      {
        return Helper.IsNumericValue(this.Expression) ? Double.Parse(this.Expression) : Double.NaN;
      }
      catch
      {
        return Double.NaN;
      }
    }
  }
}
