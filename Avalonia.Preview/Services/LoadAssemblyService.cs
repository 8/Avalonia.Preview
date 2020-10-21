using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface ILoadAssemblyService
  {
    string File { get; set; }
  }
  
  public class LoadAssemblyService : ReactiveObject, ILoadAssemblyService
  {
    readonly BehaviorSubject<string> file = new BehaviorSubject<string>(null); 
    public string File
    {
      get => this.file.Value;
      set => this.file.OnNext(value);
    }

    IDisposable subscription;
    
    public LoadAssemblyService(
      IAssemblyService assemblyService,
      IFileWatcherService fileWatcherService)
    {
      this.subscription =
        this.file
          .CombineLatest(Observable.Return(string.Empty).Merge(fileWatcherService.FileChanged), 
            (file, fileChanged) => new {file, fileChanged})
        .Subscribe(change =>
        {
          if (change.file != null)
          {
            var target = CopyFile(change.file);
            assemblyService.LoadAssemblyFromPath(target);
          }
        });
    }

    public string GetTargetFilePath(string sourceFilePath)
    {
      string origName = Path.GetFileNameWithoutExtension(sourceFilePath);
      string extension = Path.GetExtension(sourceFilePath);
      Guid g = Guid.NewGuid();
      string fileName = $"{origName}-{g}{extension}";
      string directory = Path.GetTempPath();
      return Path.Combine(directory, fileName);
    }

    public string CopyFile(string file)
    {
      var target = GetTargetFilePath(file);
      System.IO.File.Copy(file, target);
      return target;
    }
  }
}