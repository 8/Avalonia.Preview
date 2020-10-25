using System.Reflection;

namespace Avalonia.Preview.Models
{
  public class LoadedAssemblyModel
  {
    public Assembly Assembly { get; }

    public LoadedAssemblyModel(Assembly assembly)
    {
      Assembly = assembly;
    }
  }
}