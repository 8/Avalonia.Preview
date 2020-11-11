using System;
using System.Linq;
using DynamicData;

namespace Avalonia.Preview.Services
{
  public interface IRecentFileService
  {
    SourceList<string> RecentFiles { get; }
  }

  public class RecentFileService : IRecentFileService
  {
    public SourceList<string> RecentFiles { get; }
      = new SourceList<string>();
    
    public RecentFileService(ISettingsService settingsService)
    {
      this.RecentFiles.AddRange(settingsService.Settings.RecentFiles);

      this.RecentFiles.Connect().Subscribe(_ =>
      {
        settingsService.Settings.RecentFiles = this.RecentFiles.Items.ToArray();
        settingsService.Save();
      });
    }

  }
}