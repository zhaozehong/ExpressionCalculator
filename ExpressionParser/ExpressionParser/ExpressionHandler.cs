using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.ExpressionParser
{
  public class ExpressionHandler : IExpressionParser, IExpressionCalculator
  {
    public ExpressionHandler(String expression) { this.Expression = Regex.Replace(expression ?? String.Empty, " ", ""); }
    public Double Calculate(List<String> variableValues)
    {
      try
      {
        if (variableValues == null || this.VariableNames.Count != variableValues.Count || variableValues.Any(p => !Helper.IsNumericValue(p)))
          return Double.NaN;

        var replacedExpression = this.Expression;
        var newValues = variableValues.Select(p => p.TrimStart('+')).Select(p => p[0] == '-' ? String.Format("({0})", p) : p).ToList();
        for (int i = 0; i < this.VariableNames.Count; i++)
        {
          replacedExpression = Regex.Replace(replacedExpression, String.Format(@"\b{0}\b", this.VariableNames[i]), newValues[i]);
        }
        var expressionUnit = Parse(replacedExpression);
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
        if (String.IsNullOrWhiteSpace(this.Expression))
          return new List<String>();

        //var chars = System.IO.Path.GetInvalidFileNameChars();
        //if (object.ReferenceEquals(chars, this.Expression))
        //  return new List<String>();

        var functionNames = Enum.GetNames(typeof(SimpleFunctions)).Select(p => p.ToUpper()).ToList();
        var complexFunctionNames = Enum.GetNames(typeof(ComplexFunctions)).Select(p => p.ToUpper()).ToList();
        var variableNames = this.Expression.Split(SupportOperators, StringSplitOptions.RemoveEmptyEntries);
        return variableNames.Where(p => !functionNames.Contains(p.ToUpper()) && !complexFunctionNames.Contains(p.ToUpper()) && !Helper.IsNumericValue(p)).Distinct().ToList();
      }
      catch
      {
        return new List<String>();
      }
    }

    #region Parse
    public static IComputeUnit Parse(String replacedExpression)
    {
      var normalizedExpression = Normalize(replacedExpression);

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

      var expression = strExpression.TrimStart('+'); // remove prefix '+';
      while (Regex.IsMatch(expression, @"^[([{]") && Regex.IsMatch(expression, @"[)\]}]$") && Helper.GetSubExpression(expression) == expression)
      {
        expression = expression.Substring(1, expression.Length - 2).TrimStart('+'); // remove unnecessary bounded "()";
      }
      if (String.IsNullOrWhiteSpace(expression))
        return "0";

      if (Helper.IsNumericValue(expression))
        return expression;
      if (expression[0] != '+' && expression[0] != '-')
        return expression;

      // ZEHONG: maybe here is redundant, add a prefix "0" is no problem I think////////////////////////// 
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
      ///////////////////////////////////////////////////////////////////////////////////////////////

      return "0" + expression;
    }

    private static Boolean TrySimpleParse(String normalizedExpression, out String strExpression, out SimpleFunctions function)
    {
      function = SimpleFunctions.None;
      strExpression = normalizedExpression;

      /***************************************************
       * NOTE: now it has 2 classified conditions:
       * 1. sub != origin
       * 2. sub == orgin
       ***************************************************/

      // 1.sub != origin
      var subExpression = Helper.GetSubExpression(strExpression);
      if (subExpression != strExpression)
        return true;

      // 2. sub == origin
      /***************************************************
       * NOTE: now it has 3 potential conditions:
       * 1. started with number
       * 2. started with left bracket --- [Impossible]
       * 3. start with string which is function name
       * 
       * so, actually it has 2 classified conditions:
       * 1. started with number: will return true to continue with search
       * 2. started with char: to get the function name & parameter
       ***************************************************/

      // 2.1 started with number
      if (Regex.IsMatch(strExpression, @"^[0-9]")) // should never started with +/-, since it's normalized
        return true;

      // 2.2 started with char
      var match = Regex.Match(strExpression, @"[([{]");
      if (!match.Success || match.Index == 0)
        throw new Exception(String.Format("TrySimpleParse Expression: {0}", strExpression));

      // get the simple function name
      var expression1 = strExpression.Substring(0, match.Index).ToUpper();
      var functionName = Enum.GetNames(typeof(SimpleFunctions)).FirstOrDefault(p => p.ToUpper() == expression1);
      if (functionName == null)// not a simple function
        return false;

      function = (SimpleFunctions)Enum.Parse(typeof(SimpleFunctions), functionName);
      strExpression = strExpression.Substring(functionName.Length + 1, strExpression.Length - functionName.Length - 2);
      return true;
    }
    private static Boolean TryComplexParse(String normalizedExpression, out String strExpression, out ComplexFunctions complexFunction)
    {
      complexFunction = ComplexFunctions.None;
      strExpression = normalizedExpression;

      /***************************************************
      * NOTE: now it has 2 classified conditions:
      * 1. sub != origin
      * 2. sub == orgin
      ***************************************************/

      // 1.sub != origin
      var subExpression = Helper.GetSubExpression(strExpression);
      if (subExpression != strExpression)
        return false;

      // 2. sub == origin
      /***************************************************
       * NOTE: now it has 3 potential conditions:
       * 1. started with number
       * 2. started with left bracket --- [Impossible]
       * 3. start with string which is function name
       * 
       * so, actually it has 2 classified conditions:
       * 1. started with number
       * 2. started with char
       ***************************************************/

      // 2.1 started with number
      if (Regex.IsMatch(strExpression, @"^[0-9]"))
        return false;

      // 2.2 started with char
      var match = Regex.Match(strExpression, @"[([{]");
      if (!match.Success)
        throw new Exception(String.Format("TryComplexParse Expression: {0}", strExpression));

      // get the complex function name
      var expression1 = strExpression.Substring(0, match.Index).ToUpper();
      var functionName = Enum.GetNames(typeof(ComplexFunctions)).FirstOrDefault(p => p.ToUpper() == expression1);
      if (functionName == null) // not a complex function
        return false;

      complexFunction = (ComplexFunctions)Enum.Parse(typeof(ComplexFunctions), functionName);
      strExpression = strExpression.Substring(functionName.Length + 1, strExpression.Length - functionName.Length - 2);
      return true;
    }
    #endregion

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
}
