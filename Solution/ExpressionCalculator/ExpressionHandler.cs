using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Solution.ExpressionCalculator
{
  public class ExpressionHandler : IExpressionParser, IExpressionCalculator
  {
    public ExpressionHandler(String expression) { this.Expression = expression; }
    public Double Calculate(List<String> variableValues)
    {
      try
      {
        if (variableValues == null || this.VariableNames.Count != variableValues.Count || variableValues.Any(p => !Helper.IsNumericValue(p)))
          return Double.NaN;

        var strNumericExpression = Regex.Replace(this.Expression, " ", "");
        var newValues = variableValues.Select(p => p.TrimStart('+')).Select(p => p[0] == '-' ? String.Format("({0})", p) : p).ToList();
        for (int i = 0; i < this.VariableNames.Count; i++)
        {
          strNumericExpression = Regex.Replace(strNumericExpression, String.Format(@"\b{0}\b", this.VariableNames[i]), newValues[i]);
        }
        var expressionUnit = NumericCalculator.Parse(strNumericExpression);
        if (expressionUnit != null)
          return expressionUnit.Compute();
        return Double.NaN;
      }
      catch
      {
        return Double.NaN;
      }
    }

    public static Boolean Check(String expression)
    {
      try
      {
        //expression = Regex.Replace(expression, " ", "");
        //if (String.IsNullOrWhiteSpace(expression))
        //  return false;

        //var functionNames = Enum.GetNames(typeof(Functions)).Select(p => p.ToUpper()).ToList();
        //var complexFunctionNames = Enum.GetNames(typeof(ComplexFunctions)).Select(p => p.ToUpper()).ToList();
        //var variableNames = expression.Split(supportOperators, StringSplitOptions.RemoveEmptyEntries);
        //return variableNames.Where(p => !functionNames.Contains(p.ToUpper()) && !complexFunctionNames.Contains(p.ToUpper()) && !ApplicationHelper.IsNumericValue(p)).Distinct().ToList();

        return true;
      }
      catch
      {
        return false;
      }
    }

    private List<String> GetVariables()
    {
      try
      {
        var expression = Regex.Replace(this.Expression, " ", "");
        if (String.IsNullOrWhiteSpace(expression))
          return new List<String>();

        //var chars = System.IO.Path.GetInvalidFileNameChars();
        //if (object.ReferenceEquals(chars, expression))
        //  return new List<String>();

        var functionNames = Enum.GetNames(typeof(SimpleFunctions)).Select(p => p.ToUpper()).ToList();
        var complexFunctionNames = Enum.GetNames(typeof(ComplexFunctions)).Select(p => p.ToUpper()).ToList();
        var variableNames = expression.Split(SupportOperators, StringSplitOptions.RemoveEmptyEntries);
        return variableNames.Where(p => !functionNames.Contains(p.ToUpper()) && !complexFunctionNames.Contains(p.ToUpper()) && !Helper.IsNumericValue(p)).Distinct().ToList();
      }
      catch
      {
        return new List<String>();
      }
    }

    public String Expression { get; private set; }
    public List<string> VariableNames
    {
      get
      {
        if (_variableNames == null)
          _variableNames = GetVariables();
        return _variableNames;
      }
    }

    private List<String> _variableNames = null;

    private static char[] SupportOperators = new[] { '+', '-', '*', '/', '%', '(', '[', '{', ')', ']', '}', ',' };
  }
  public class NumericCalculator
  {
    public static IComputeUnit Parse(String strNumericExpression)
    {
      var normalizedExpression = Normalize(strNumericExpression);
      if (Helper.IsNumericValue(normalizedExpression))
        return new NumericUnit(normalizedExpression);

      String strExpression;
      SimpleFunctions simplefunction;
      if (TrySimpleParse(normalizedExpression, out strExpression, out simplefunction))
        return new SimpleUnit(strExpression, simplefunction);

      ComplexFunctions complexFunction;
      if (TryComplexParse(normalizedExpression, out strExpression, out complexFunction))
        return new ComplexUnit(strExpression, complexFunction);

      return null;
    }
    public static String Normalize(String strExpression)
    {
      if (String.IsNullOrWhiteSpace(strExpression))
        return "0";

      var expression = Regex.Replace(strExpression, " ", ""); // remove empty char
      expression = expression.TrimStart('+'); // remove prefix '+';
      while (Regex.IsMatch(expression, @"^[([{]") && Regex.IsMatch(expression, @"[)\]}]$") && Helper.GetSubExpression(expression) == expression)
      {
        expression = expression.Substring(1, expression.Length - 2); // remove unnecessary bounded "()";
      }

      if (Helper.IsNumericValue(expression))
        return expression;
      if (expression[0] != '+' && expression[0] != '-')
        return expression;

      var matchs = Regex.Matches(expression, @"[\+\-*/%\,]").OfType<Match>().ToList(); // '+', '-', '*', '/', '%' , ','
      if (matchs.Count >= 2)
      {
        var temp = expression.Substring(0, matchs[1].Index);
        if (Helper.IsNumericValue(temp)) // if expression starts with '+' or '-', add "()" to it.
        {
          expression = expression.Insert(matchs[1].Index, ")");
          expression = expression.Insert(0, "(");
          return expression;
        }
      }

      return "0" + expression;
    }

    private static Boolean TrySimpleParse(String normalizedExpression, out String strExpression, out SimpleFunctions function)
    {
      function = SimpleFunctions.None;
      strExpression = String.Copy(normalizedExpression);
      if (Regex.IsMatch(strExpression, @"^[0-9]"))
        return true;

      var subExpression = Helper.GetSubExpression(strExpression);
      if (subExpression != strExpression)
        return true;

      var match = Regex.Match(strExpression, @"[([{]");
      if (!match.Success)
        return false;

      if (match.Index == 0)
        return true;

      var expression1 = strExpression.Substring(0, match.Index).ToUpper();
      var functionName = Enum.GetNames(typeof(SimpleFunctions)).FirstOrDefault(p => p.ToUpper() == expression1);

      if (functionName == null)
        return false;

      function = (SimpleFunctions)Enum.Parse(typeof(SimpleFunctions), functionName);
      strExpression = strExpression.Substring(functionName.Length + 1, strExpression.Length - functionName.Length - 2);
      return true;
    }
    private static Boolean TryComplexParse(String normalizedExpression, out String strExpression, out ComplexFunctions complexFunction)
    {
      complexFunction = ComplexFunctions.None;
      strExpression = String.Copy(normalizedExpression);
      if (Regex.IsMatch(strExpression, @"^[0-9]"))
        return false;

      var subExpression = Helper.GetSubExpression(strExpression);
      if (subExpression != strExpression)
        return false;

      var match = Regex.Match(strExpression, @"[([{]");
      if (!match.Success)
        return false;

      var expression1 = strExpression.Substring(0, match.Index).ToUpper();
      var functionName = Enum.GetNames(typeof(ComplexFunctions)).FirstOrDefault(p => p.ToUpper() == expression1);
      if (functionName == null)
        return false;

      complexFunction = (ComplexFunctions)Enum.Parse(typeof(ComplexFunctions), functionName);
      strExpression = strExpression.Substring(functionName.Length + 1, strExpression.Length - functionName.Length - 2);
      return true;
    }
  }
}
