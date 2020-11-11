using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface IControlService
  {
    Control Control { get; }
  }

  public class ControlService : ReactiveObject, IControlService
  {
    Control _control;
    IDisposable _subscription;

    public Control Control
    {
      get => this._control;
      set => this.RaiseAndSetIfChanged(ref _control, value);
    }

    T CreateInstance<T>(Type controlType) where T : class =>
      // controlType is not null ? Activator.CreateInstance(controlType) as T : null;
      controlType?.GetConstructor(new Type[0])?.Invoke(null) as T;
  
    public ControlService(IControlTypeService controlTypeService)
    {
      this._subscription = controlTypeService.WhenAnyValue(s => s.SelectedControlType)
        .Do(t => Debug.WriteLine($"Type changed to '{t?.FullName}' with HostContext: '{t?.Assembly.HostContext}'"))
        .ObserveOn(AvaloniaScheduler.Instance)
        .SubscribeOn(AvaloniaScheduler.Instance)
        .Select(CreateInstance<Control>)
        .Subscribe(c => this.Control = c);
    }
  }
}