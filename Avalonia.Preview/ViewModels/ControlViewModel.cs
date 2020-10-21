using System;

namespace Avalonia.Preview.ViewModels
{
  public class ControlViewModel
  {
    public string Name { get; set; }
    public Type ControlType { get; set; }
    public override string ToString() => this.Name;
  }
}
