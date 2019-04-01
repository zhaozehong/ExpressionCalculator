using System;
using System.Collections.Generic;

namespace Solution.ExpressionCalculator
{
  public interface IExpressionParser : IExpression
  {
    List<String> VariableNames { get; }
  }
}
