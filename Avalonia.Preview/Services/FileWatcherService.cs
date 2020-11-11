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
    FileSystemWatcher _watcher;
    IDisposable _watcherChangedSubscription;
    readonly Subject<string> _fileChangedSubject = new Subject<string>();
    public IObservable<string> FileChanged => this._fileChangedSubject.AsObservable();

    string _watchedFile;
    public string WatchedFile
    {
      get => this._watchedFile;
      set
      {
        _watchedFile = value;
        this.WatchFile(value);
      }
    }

    void WatchFile(string file)
    {
      this._watcher?.Dispose();
      this._watcher = null;
      this._watcherChangedSubscription?.Dispose();
      this._watcherChangedSubscription = null;

      if (file != null)
      {
        var watcher = this._watcher = CreateWatcher(file);

        var observable = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
          h => watcher.Changed += h,
          h => watcher.Changed -= h);

        this._watcherChangedSubscription = observable
          .ObserveOn(AvaloniaScheduler.Instance)
          .SubscribeOn(AvaloniaScheduler.Instance)
          .Subscribe(change => this._fileChangedSubject.OnNext(change.EventArgs.FullPath));
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