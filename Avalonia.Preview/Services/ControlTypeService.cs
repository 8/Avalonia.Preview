using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Controls;
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
    readonly SourceList<Type> controlTypesSource = new SourceList<Type>();
    public IObservableList<Type> ControlTypes => this.controlTypesSource.AsObservableList();

    Type selectedControlSelectedControlType;
    IDisposable subscription;

    public Type SelectedControlType
    {
      get => this.selectedControlSelectedControlType;
      set => this.RaiseAndSetIfChanged(ref this.selectedControlSelectedControlType, value);
    }

    static Type[] LoadTypes(Assembly assembly)
      => assembly?.GetTypes()
          .Where(type => type.IsSubclassOf(typeof(Control)))
          .ToArray();

    public ControlTypeService(IAssemblyService assemblyService)
    {
      this.subscription = assemblyService.WhenAnyValue(s => s.Assembly)
        .Select(LoadTypes)
        .Where(types => types != null)
        .Subscribe(controlTypes =>
        {
          this.controlTypesSource.Clear();
          this.controlTypesSource.AddRange(controlTypes);
        });
    }
  }
}