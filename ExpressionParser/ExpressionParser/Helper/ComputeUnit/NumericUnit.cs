using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.ExpressionParser
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
