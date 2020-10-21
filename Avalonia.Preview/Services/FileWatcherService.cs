using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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

    public FileWatcherService()
    {
    }

    void WatchFile(string file)
    {
      this.watcher?.Dispose();
      this.watcher = null;

      this.watcher = CreateWatcher(file);
      this.watcher.Changed += (o, e) => this.fileChangedSubject.OnNext(e.FullPath);
    }

    FileSystemWatcher CreateWatcher(string filePath)
    {
      var fileName = Path.GetFileName(filePath);
      var directory = Path.GetDirectoryName(filePath) ?? throw new ArgumentException($"{filePath} does not contain a directory!");
      return new FileSystemWatcher
      {
        Path = directory,
        EnableRaisingEvents = true,
        Filter = fileName,
        NotifyFilter = NotifyFilters.LastWrite,
      };
    }
  }
}