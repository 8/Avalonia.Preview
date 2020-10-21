using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface ILoadAssemblyService
  {
    // void LoadAssembly(string file);
    string File { get; set; }
  }
  
  public class LoadAssemblyService : ReactiveObject, ILoadAssemblyService
  {
    string file;
    public string File
    {
      get => this.file;
      set => this.RaiseAndSetIfChanged(ref file, value);
    }

    IDisposable subscription;
    
    public LoadAssemblyService(
      IAssemblyService assemblyService,
      IFileWatcherService fileWatcherService)
    {
      this.subscription =
        this.WhenAnyValue(vm => vm.File)
          .CombineLatest(Observable.Return(string.Empty).Merge(fileWatcherService.FileChanged), 
            (file, fileChanged) => new {file, fileChanged})
        .Subscribe(change =>
        {
          if (change.file != null)
            assemblyService.LoadAssemblyFromPath(file);
        });
    }

    
  }
}