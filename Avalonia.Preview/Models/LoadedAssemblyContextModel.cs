using System.Collections.Generic;
using System.Runtime.Loader;

namespace Avalonia.Preview.Models
{
  public class LoadedAssemblyContextModel
  {
    public AssemblyLoadContext Context { get; }
    public LoadedAssemblyModel MainAssembly { get; }
    public List<LoadedAssemblyModel> AdditionalAssemblies { get; }
      = new List<LoadedAssemblyModel>();

    public LoadedAssemblyContextModel(
      AssemblyLoadContext context,
      LoadedAssemblyModel mainAssembly)
    {
      this.Context = context;
      this.MainAssembly = mainAssembly;
    }
  }
}