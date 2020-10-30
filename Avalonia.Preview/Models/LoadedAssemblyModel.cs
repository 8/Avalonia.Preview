using System.Reflection;

namespace Avalonia.Preview.Models
{
  public class LoadedAssemblyModel
  {
    public Assembly Assembly { get; }
    public string Source { get; }
    public string Target { get; }

    public LoadedAssemblyModel(
      Assembly assembly,
      string source,
      string target)
    {
      Assembly = assembly;
      Source = source;
      Target = target;
    }
  }
}