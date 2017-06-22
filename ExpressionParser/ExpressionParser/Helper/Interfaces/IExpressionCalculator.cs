using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.ExpressionParser
{
  public interface IExpressionCalculator : IExpression
  {
    Double Calculate(List<String> variableValues);
  }
}
