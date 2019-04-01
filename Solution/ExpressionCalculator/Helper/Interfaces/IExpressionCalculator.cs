using System;
using System.Collections.Generic;

namespace Solution.ExpressionCalculator
{
  public interface IExpressionCalculator : IExpression
  {
    Double Calculate(List<String> variableValues);
  }
}
