using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.ExpressionParser
{
  public interface IExpressionParser : IExpression
  {
    List<String> VariableNames { get; }
  }
}
