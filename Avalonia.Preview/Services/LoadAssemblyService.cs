﻿using System;
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
using FluentAssertions;

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
    }

    LoadedAssemblyContextModel CreateLoadedAssembly(string source, string file)
    {
      var context = new AssemblyLoadContext("ControlContext", true);
      context.Resolving += TryResolveAssembly;
      var mainAssembly = new LoadedAssemblyModel(context.LoadFromAssemblyPath(file), source, file);
      return new LoadedAssemblyContextModel(context, mainAssembly);
    }

    Assembly TryResolveAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
    {
      var loadedContext = this.LoadedAssemblyContext;
      Assembly ret = null;

      if (loadedContext?.Context != context)
      {
        context.Resolving -= TryResolveAssembly;
      }
      else if (loadedContext != null)
      {
        Debug.WriteLine($"Trying to resolve '{assemblyName.Name}'");
        string folder = Path.GetDirectoryName(loadedContext.MainAssembly.Source);
        string fileName = $"{assemblyName.Name}.dll";
        string source = Path.Combine(folder, fileName);
        var target = Retry(() => CopyFile(source), 3, TimeSpan.FromMilliseconds(200));
        var assembly = loadedContext.Context.LoadFromAssemblyPath(target);
        var loadedAssembly = new LoadedAssemblyModel(assembly, source, target);
        loadedContext.AdditionalAssemblies.Add(loadedAssembly);
        ret = assembly;
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
      dictionary.Clear();
    }
    
    void LoadFile(string file)
    {
      if (file != null)
      {
        var target = Retry(() => CopyFile(file), 3, TimeSpan.FromMilliseconds(200));
        if (target != null)
        {
          ResetDynamicDataCache();
          
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