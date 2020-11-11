using System.Runtime.Serialization;
using ReactiveUI;

namespace Avalonia.Preview.Models
{
  public class SettingsModel : ReactiveObject
  {
    string[] _recentFiles;

    [DataMember]
    public string[] RecentFiles
    {
      get => _recentFiles;
      set => this.RaiseAndSetIfChanged(ref _recentFiles, value);
    }

    public SettingsModel()
    {
      this.RecentFiles = new string[0];
    }
  }
}