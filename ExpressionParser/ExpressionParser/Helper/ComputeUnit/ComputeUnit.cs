using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.ExpressionParser
{
  public abstract class ComputeUnit : IComputeUnit
  {
    public ComputeUnit(String strExpression)
    {
      this.Expression = NumericCalculator.Normalize(strExpression);
    }

    public abstract double Compute();
    public String Expression { get; private set; }
  }
}
