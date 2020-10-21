using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Threading;

namespace Avalonia.Preview.Services
{
  public interface IFileWatcherService
  {
    string WatchedFile { get; set; }
    IObservable<string> FileChanged { get; }
  }

  public class FileWatcherService : IFileWatcherService
  {
    FileSystemWatcher watcher;
    IDisposable watcherChangedSubscription;
    readonly Subject<string> fileChangedSubject = new Subject<string>();
    public IObservable<string> FileChanged => this.fileChangedSubject.AsObservable();

    string watchedFile;
    public string WatchedFile
    {
      get => this.watchedFile;
      set
      {
        watchedFile = value;
        this.WatchFile(value);
      }
    }

    void WatchFile(string file)
    {
      this.watcher?.Dispose();
      this.watcher = null;
      this.watcherChangedSubscription?.Dispose();
      this.watcherChangedSubscription = null;

      if (file != null)
      {
        this.watcher = CreateWatcher(file);

        var observable = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
          h => watcher.Changed += h,
          h => watcher.Changed -= h);

        this.watcherChangedSubscription = observable
            .Subscribe(change => this.fileChangedSubject.OnNext(change.EventArgs.FullPath));
      }
    }

    FileSystemWatcher CreateWatcher(string filePath)
    {
      var fileName = Path.GetFileName(filePath);
      var directory = Path.GetDirectoryName(filePath) ??
                      throw new ArgumentException($"{filePath} does not contain a directory!");
      return new FileSystemWatcher
      {
        Path = directory,
        Filter = fileName,
        NotifyFilter = NotifyFilters.LastWrite,
        EnableRaisingEvents = true,
      };
    }
  }
}