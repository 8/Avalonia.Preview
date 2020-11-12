using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface IThemeService
  {
    SourceList<string> Themes { get; }
    string SelectedTheme { get; set; }
  }
  
  public class ThemeService : ReactiveObject, IThemeService
  {
    public SourceList<string> Themes { get; }
      = new SourceList<string>();

    string _selectedTheme;
    public string SelectedTheme
    {
      get => _selectedTheme;
      set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
    }
    
    public ThemeService()
    {
      this.Themes.AddRange(new [] { "Light", "Dark" });
      this.SelectedTheme = this.Themes.Items.FirstOrDefault();
    }
  }
}