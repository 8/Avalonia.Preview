using System.Linq;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface IFileService
  {
    string SelectedFile { get; set; }
  }

  public class FileService : ReactiveObject, IFileService
  {
    string _selectedFile;
    public string SelectedFile
    {
      get => _selectedFile;
      set => this.RaiseAndSetIfChanged(ref _selectedFile, value);
    }

    public FileService(IRecentFileService recentFileService)
    {
      this.SelectedFile = recentFileService.RecentFiles.Items.FirstOrDefault();
    }
  }
}