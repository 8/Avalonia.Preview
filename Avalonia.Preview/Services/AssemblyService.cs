using System.Reflection;
using System.Runtime.Loader;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface IAssemblyService
  {
    Assembly Assembly { get; set; }

    void LoadAssemblyFromPath(string file);
  }

  public class AssemblyService : ReactiveObject, IAssemblyService
  {
    Assembly _assembly;

    public Assembly Assembly
    {
      get => this._assembly;
      set => this.RaiseAndSetIfChanged(ref _assembly, value);
    }

    public void LoadAssemblyFromPath(string file)
    {
      AssemblyLoadContext context = new AssemblyLoadContext("ControlContext", true);
      this.Assembly = context.LoadFromAssemblyPath(file);
    }
  }
}