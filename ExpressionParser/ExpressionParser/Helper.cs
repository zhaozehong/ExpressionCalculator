using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.ExpressionParser
{
  public static class Helper
  {
    public static Boolean IsNumericValue(String strValue)
    {
      return Regex.IsMatch(strValue, @"^-?\d+\.?\d*$");
    }
    public static Boolean IsEmail(String strValue)
    {
      return Regex.IsMatch(strValue, @"^\s*([A-Za-z\d]+[\.\w-]*@([A-Za-z\d]+[\w-]*\.)+[A-Za-z]{2,})\s*$");
    }
    public static String GetSubExpression(String strExpression)
    {
      if (String.IsNullOrWhiteSpace(strExpression))
        return null;

      var leftIndexs = Regex.Matches(strExpression, @"[([{]").OfType<Match>().Select(m => m.Index).ToList();
      var rightIndexs = Regex.Matches(strExpression, @"[)\]}]").OfType<Match>().Select(m => m.Index).ToList();
      if (leftIndexs.Count != rightIndexs.Count)
        return null;

      var stopIndexs = rightIndexs.Where((r, index) => leftIndexs.Count(l => l < r) == (index + 1));
      return stopIndexs.Any() ? strExpression.Substring(0, stopIndexs.FirstOrDefault() + 1) : strExpression;
    }
  }
}
