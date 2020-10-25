using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Subjects;
using System.Runtime.Loader;
using System.Threading;
using ReactiveUI;
using Avalonia.Preview.Models;

namespace Avalonia.Preview.Services
{
  public interface ILoadAssemblyService
  {
    string File { get; set; }
    LoadedAssemblyModel LoadedAssembly { get; }
  }

  public class LoadAssemblyService : ReactiveObject, ILoadAssemblyService
  {
    readonly BehaviorSubject<string> _file = new BehaviorSubject<string>(null);
    public string File
    {
      get => this._file.Value;
      set => this._file.OnNext(value);
    }

    LoadedAssemblyModel _loadedAssembly;
    public LoadedAssemblyModel LoadedAssembly
    {
      get => _loadedAssembly;
      set => this.RaiseAndSetIfChanged(ref _loadedAssembly, value);
    }

    readonly List<IDisposable> _subscriptions = new List<IDisposable>();

    public LoadAssemblyService(
      IFileWatcherService fileWatcherService)
    {
      this._subscriptions.Add(this._file.Subscribe(file =>
      {
        fileWatcherService.WatchedFile = file;
        LoadFile(file);
      }));
      this._subscriptions.Add(fileWatcherService.FileChanged.Subscribe(LoadFile));
    }

    LoadedAssemblyModel CreateLoadedAssembly(string file)
    {
      var context = new AssemblyLoadContext("ControlContext", true);
      context.Resolving += (loadContext, name) =>
      {
        Debug.WriteLine($"Trying to resolve '{name}'");
        return null;
      };
      
      return new LoadedAssemblyModel(context.LoadFromAssemblyPath(file));
    }
    
    void LoadFile(string file)
    {
      if (file != null)
      {
        var target = Retry(() => CopyFile(file), 3, TimeSpan.FromMilliseconds(200));
        if (target != null)
          this.LoadedAssembly = this.CreateLoadedAssembly(target);
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