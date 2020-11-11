using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using ReactiveUI;
using Avalonia.Preview.Models;

namespace Avalonia.Preview.Services
{
  public interface ILoadAssemblyService
  {
    string File { get; set; }
    LoadedAssemblyContextModel LoadedAssemblyContext { get; }
  }

  public class LoadAssemblyService : ReactiveObject, ILoadAssemblyService
  {
    readonly BehaviorSubject<string> _file = new BehaviorSubject<string>(null);
    public string File
    {
      get => this._file.Value;
      set => this._file.OnNext(value);
    }

    LoadedAssemblyContextModel _loadedAssemblyContext;
    public LoadedAssemblyContextModel LoadedAssemblyContext
    {
      get => _loadedAssemblyContext;
      set => this.RaiseAndSetIfChanged(ref _loadedAssemblyContext, value);
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

      AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
        ResolveAndAdd(this.LoadedAssemblyContext, assemblyName);
    }

    LoadedAssemblyContextModel CreateLoadedAssembly(string source, string file)
    {
      var context = new AssemblyLoadContext("ControlContext", true);
      context.Resolving += TryResolveAssembly;
      var mainAssembly = new LoadedAssemblyModel(context.LoadFromAssemblyPath(file), source, file);

      var loadedAssemblyContext = new LoadedAssemblyContextModel(context, mainAssembly);

      return loadedAssemblyContext;
    }

    string GetAssemblyFileName(LoadedAssemblyContextModel loadedContext, AssemblyName assemblyName)
    {
      string folder = Path.GetDirectoryName(loadedContext.MainAssembly.Source);
      string fileName = $"{assemblyName.Name}.dll";
      string source = Path.Combine(folder, fileName);
      return source;
    }
    
    Assembly ResolveAndAdd(LoadedAssemblyContextModel loadedContext, AssemblyName assemblyName)
    {
      var source = GetAssemblyFileName(loadedContext, assemblyName);
      var target = Retry(() => CopyFile(source), 3, TimeSpan.FromMilliseconds(200));
      var assembly = target is not null ? loadedContext.Context.LoadFromAssemblyPath(target) : null;
      if (assembly is not null)
      {
        var loadedAssembly = new LoadedAssemblyModel(assembly, source, target);
        loadedContext.AdditionalAssemblies.Add(loadedAssembly);
      }
      return assembly;
    }

    Assembly TryResolveAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
    {
      var loadedContext = this.LoadedAssemblyContext;
      Assembly ret = null;

      if (loadedContext?.Context != context)
      {
        context.Resolving -= TryResolveAssembly;
      }
      else 
      if (loadedContext != null)
      {
        Debug.WriteLine($"Trying to resolve '{assemblyName.Name}'");

        ret = ResolveAndAdd(loadedContext, assemblyName);;
      }

      return ret;
    }

    static void ResetDynamicDataCache()
    {
      /* get the internal type */
      var type = Assembly.Load("DynamicData").GetTypes()
        .FirstOrDefault(t => t.FullName == "DynamicData.Binding.ObservablePropertyFactoryCache");
      
      /* get static instance */
      var instance = type.GetField("Instance").GetValue(null);
      var factoriesFieldInfo = type.GetField("_factories", BindingFlags.Instance | BindingFlags.NonPublic);

      /* get the internal dictary */
      var dictionary = (ConcurrentDictionary<string, object>) factoriesFieldInfo.GetValue(instance);
      
      /* clear the internal cache */
      Debug.WriteLine($"{DateTime.Now:hh:mm:ss} Workaround: Resetting DynamicDataCache with {dictionary.Count} items");
      
      dictionary.Clear();
    }

    void DisposeLoadedAssemblyContext()
    {
      var context = this.LoadedAssemblyContext;
      if (context != null)
      {
        context.Context.Resolving -= TryResolveAssembly;
        context.Context.Unload();
      }
      this.LoadedAssemblyContext = null;
    }
    
    void LoadFile(string file)
    {
      if (file != null)
      {
        var target = Retry(() => CopyFile(file), 3, TimeSpan.FromMilliseconds(200));
        if (target != null)
        {
          this.DisposeLoadedAssemblyContext();
          
          ResetDynamicDataCache();

          Debug.WriteLine($"{DateTime.Now:hh:mm:ss} Loading assembly {file}");
          this.LoadedAssemblyContext = this.CreateLoadedAssembly(file, target);
        }
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