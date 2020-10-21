using System.Linq;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface IFileService
  {
    string File { get; set; }
  }
  
  public class FileService : ReactiveObject, IFileService
  {
    string file;
    public string File
    {
      get => file;
      set => this.RaiseAndSetIfChanged(ref file, value);
    }

    public FileService(IRecentFileService recentFileService)
    {
      this.File = recentFileService.RecentFiles.Items.FirstOrDefault();
    }
  }
}