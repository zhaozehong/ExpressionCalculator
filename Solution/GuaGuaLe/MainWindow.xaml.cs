using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GuaGuaLe
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }


    private String CreateRandomString(int length)
    {
      var stringBuilder = new StringBuilder(length);
      var randomObj = new Random();
      for (int i = 0; i < length; i++)
      {
        var iRandom = randomObj.Next(0, 62);
        stringBuilder.Append(CharArray[iRandom]);

        stringBuilder
      }
      return stringBuilder.ToString();
    }


    private static string[] CharArray = new String[]
    {
      "0", "1", "2", "3","4","5", "6", "7", "8", "9",
      "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
      "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };
  }
}
