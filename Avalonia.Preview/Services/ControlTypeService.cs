using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;

namespace Avalonia.Preview.Services
{
  public interface IControlTypeService
  {
    IObservableList<Type> ControlTypes { get; }
    Type SelectedControlType { get; set; }
  }

  public class ControlTypeService : ReactiveObject, IControlTypeService
  {
    readonly SourceList<Type> _controlTypesSource = new SourceList<Type>();
    public IObservableList<Type> ControlTypes => this._controlTypesSource.AsObservableList();

    Type _selectedControlSelectedControlType;
    IDisposable _subscription;

    public Type SelectedControlType
    {
      get => this._selectedControlSelectedControlType;
      set => this.RaiseAndSetIfChanged(ref this._selectedControlSelectedControlType, value);
    }

    static Type[] LoadTypes(Assembly assembly)
      => assembly?.GetTypes()
        .Where(type => type.IsSubclassOf(typeof(Control)))
        .ToArray();

    public ControlTypeService(ILoadAssemblyService assemblyService)
    {
      this._subscription = assemblyService.WhenAnyValue(s => s.LoadedAssemblyContext)
        .Where(m => m != null)
        .Select(m => m.MainAssembly.Assembly)
        .Select(LoadTypes)
        .Where(types => types != null)
        .Subscribe(controlTypes =>
        {
          Dispatcher.UIThread.Post(() =>
          {
            var prevSelectedTypeFullName = this.SelectedControlType?.FullName;
            
            this._controlTypesSource.Clear();
            this._controlTypesSource.AddRange(controlTypes);

            if (prevSelectedTypeFullName != null)
            {
              this.SelectedControlType = controlTypes.FirstOrDefault(type => type.FullName == prevSelectedTypeFullName);
            }
            
          });
        });
    }
  }
}