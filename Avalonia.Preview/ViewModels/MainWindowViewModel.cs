using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using DynamicData;
using DynamicData.Alias;
using Avalonia.Controls;
using Avalonia.Preview.Services;

namespace Avalonia.Preview.ViewModels
{
  public interface IMainWindowViewModel
  {
  }

  public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
  {
    string file;
    public string File
    {
      get => this.file;
      set => this.RaiseAndSetIfChanged(ref file, value);
    }

    readonly ReadOnlyObservableCollection<ControlViewModel> controls;
    public ReadOnlyObservableCollection<ControlViewModel> Controls => controls;

    ControlViewModel selectedControl;
    public ControlViewModel SelectedControl
    {
      get => this.selectedControl;
      set => this.RaiseAndSetIfChanged(ref selectedControl, value);
    }

    Type controlType;
    public Type ControlType
    {
      get => this.controlType;
      set => this.RaiseAndSetIfChanged(ref controlType, value);
    }

    Control control;
    public Control Control
    {
      get => this.control;
      set => this.RaiseAndSetIfChanged(ref control, value);
    }

    public ICommand LoadCommand { get; }

    readonly List<IDisposable> subscriptions = new List<IDisposable>();
    readonly ILoadAssemblyService assemblyService;

    public MainWindowViewModel(
      ILoadAssemblyService assemblyService,
      ILoadAssemblyService loadAssemblyService,
      IFileService fileService,
      IControlTypeService controlTypeService,
      IControlService controlService)
    {
      this.assemblyService = assemblyService;
      this.LoadCommand = ReactiveCommand.Create(() => loadAssemblyService.File = this.File);
      this.File = fileService.SelectedFile;

      this.subscriptions.Add(
        controlTypeService.ControlTypes.Connect()
          .Select(CreateControlViewModel)
          .Bind(out this.controls)
          .Subscribe()
      );

      this.subscriptions.Add(
        this.WhenAnyValue(vm => vm.SelectedControl)
          .Subscribe(vm => controlTypeService.SelectedControlType = vm?.ControlType));
      
      this.subscriptions.Add(
        controlTypeService.WhenAnyValue(s => s.SelectedControlType)
          .Subscribe(type => this.SelectedControl = this.controls.FirstOrDefault(vm => vm.ControlType == type))
        );

      this.subscriptions.Add(
         controlService.WhenAnyValue(s => s.Control)
           .Subscribe(c => this.Control = c));

      this.Control = null;
    }

    static ControlViewModel CreateControlViewModel(Type controlType)
      => new ControlViewModel { ControlType = controlType, Name = controlType.Name };
  }
}