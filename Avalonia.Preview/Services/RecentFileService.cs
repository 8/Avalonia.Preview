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

    public RecentFileService()
    {
      this.RecentFiles.Add(
        @"C:\projects\AvaloniaPreviewTest\AvaloniaPreviewTest\bin\Debug\netcoreapp5.0\AvaloniaPreviewTest.dll");
    }
  }
}