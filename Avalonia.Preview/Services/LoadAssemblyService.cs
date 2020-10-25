using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface ILoadAssemblyService
  {
    string File { get; set; }
  }

  public class LoadAssemblyService : ReactiveObject, ILoadAssemblyService
  {
    readonly IAssemblyService _assemblyService;
    readonly BehaviorSubject<string> _file = new BehaviorSubject<string>(null);
    public string File
    {
      get => this._file.Value;
      set => this._file.OnNext(value);
    }

    readonly List<IDisposable> _subscriptions = new List<IDisposable>();

    public LoadAssemblyService(
      IAssemblyService assemblyService,
      IFileWatcherService fileWatcherService)
    {
      this._assemblyService = assemblyService;
      this._subscriptions.Add(this._file.Subscribe(file =>
      {
        fileWatcherService.WatchedFile = file;
        LoadFile(file);
      }));
      this._subscriptions.Add(fileWatcherService.FileChanged.Subscribe(LoadFile));
    }

    void LoadFile(string file)
    {
      if (file != null)
      {
        var target = Retry(() => CopyFile(file), 3, TimeSpan.FromMilliseconds(200));
        this._assemblyService.LoadAssemblyFromPath(target);
      }
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

    T Retry<T>(Func<T> action, int maxRetries, TimeSpan wait) where T : class
    {
      int retry = 0;
      T result = null;
      do
      {
        try
        {
          result = action();
        }
        catch (Exception ex) { Thread.Sleep(wait); }
      } while (result == null && retry++ < maxRetries);

      return result;
    }

    public string CopyFile(string file)
    {
      var target = GetTargetFilePath(file);
      System.IO.File.Copy(file, target);
      return target;
    }
  }
}