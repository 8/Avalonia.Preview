using System.Collections;
using System.Linq;
using Xunit.Abstractions;

namespace Avalonia.Preview
{
  public static class TestOutputHelperExtensions
  {
    static string Format(object obj)
    {
      return obj switch
      {
        null => "NULL",
        string s => s,
        IEnumerable e => "[" + string.Join(", ", e.Cast<object>().Select(Format).ToArray()) + "]",
        var o => o.ToString(),
      };
    }
    
    public static void Dump(this ITestOutputHelper output, object o) => output.WriteLine(Format(o));
  }
}