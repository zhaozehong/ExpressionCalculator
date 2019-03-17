using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Zehong.CSharp.Solution.ExpressionParser
{
  class Program
  {
    static void Main(string[] args)
    {
      while (true)
      {
        Console.WriteLine("Please enter any mathematical expression:");
        var strExpression = Regex.Replace(Console.ReadLine(), " ", "");
        if (strExpression.ToLower() == "exit")
          break;

        //strExpression = "12 + sqrt((1+3)*abs(-5+1-(2+3-2-3)))*max(-abs(-1),min(-5,-3))-min(12,128)";
        //strExpression = "abs(min(a,-100))*sqrt(a*b)+c-d%e";

        var handler = new ExpressionHandler(strExpression);
        var variableValues = new List<String>();
        foreach (var variableName in handler.VariableNames)
        {
          Console.WriteLine(String.Format("Please enter value for [{0}]:", variableName));
          var variableValue = Regex.Replace(Console.ReadLine(), " ", "").TrimStart('+');
          while (!Helper.IsNumericValue(variableValue))
          {
            Console.WriteLine("Please enter a VALID value!!!");
            variableValue = Regex.Replace(Console.ReadLine(), " ", "").TrimStart('+');
          }
          variableValues.Add(variableValue);
        }

        var value = handler.Calculate(variableValues);
        Console.WriteLine(String.Format("Result:\r\n{0}\r\n", value));
      }
    }
  }
}
