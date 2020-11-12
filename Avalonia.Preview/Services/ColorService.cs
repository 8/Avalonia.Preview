using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Media;

namespace Avalonia.Preview.Services
{
  public interface IColorService
  {
    string[] Colors { get; }

    Color? GetColorFromName(string name);
  }
  
  public class ColorService : IColorService
  {
    public string[] Colors { get; }

    readonly Dictionary<string, Color> _colorLookup;
    
    public ColorService()
    {
      this.Colors = GetAllColors();
      this._colorLookup = GetColorLookup();
    }

    string[] GetAllColors() =>
      typeof(Media.Colors)
        .GetProperties(BindingFlags.Static | BindingFlags.Public)
        .Select(prop => prop.Name)
        .ToArray();

    Dictionary<string, Color> GetColorLookup() =>
      typeof(Media.Colors)
        .GetProperties(BindingFlags.Static | BindingFlags.Public)
        .Select(prop => (name: prop.Name, color: (Color)prop.GetValue(null)))
        .ToDictionary(item => item.name, item =>item.color);

    public Color? GetColorFromName(string name) 
      => this._colorLookup.TryGetValue(name, out Color c) ? (Color?)c : null;
  }
}