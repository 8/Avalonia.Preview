using System;
using System.IO;
using Avalonia.Preview.Models;
using Newtonsoft.Json;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface ISettingsService
  {
    SettingsModel Settings { get; set; }
    void Load();
    void Save();
  }

  public class SettingsService : ReactiveObject, ISettingsService
  {
    SettingsModel _settings;
    public SettingsModel Settings
    {
      get => _settings;
      set => this.RaiseAndSetIfChanged(ref _settings, value);
    }

    public SettingsService()
    {
      this.Load();
    }

    string GetBaseFolder() =>
      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Avalonia.Preview");

    string GetSettingsFile() =>
      Path.Combine(GetBaseFolder(), "settings.json");

    public void Load()
    {
      this.Settings = (Directory.Exists(GetBaseFolder()) && File.Exists(GetSettingsFile()))
        ? JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(GetSettingsFile()))
        : new SettingsModel();
    }

    public void Save() => Save(this.Settings);

    void Save(SettingsModel settings)
    {
      var folder = GetBaseFolder();
      if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);
      
      File.WriteAllText(GetSettingsFile(), JsonConvert.SerializeObject(settings));
    }
  }
}