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
    string _file;
    public string File
    {
      get => this._file;
      set => this.RaiseAndSetIfChanged(ref _file, value);
    }

    readonly ReadOnlyObservableCollection<ControlViewModel> _controls;
    public ReadOnlyObservableCollection<ControlViewModel> Controls => _controls;

    ControlViewModel _selectedControl;
    public ControlViewModel SelectedControl
    {
      get => this._selectedControl;
      set => this.RaiseAndSetIfChanged(ref _selectedControl, value);
    }

    Type _controlType;
    public Type ControlType
    {
      get => this._controlType;
      set => this.RaiseAndSetIfChanged(ref _controlType, value);
    }

    Control _control;
    public Control Control
    {
      get => this._control;
      set => this.RaiseAndSetIfChanged(ref _control, value);
    }

    public ICommand LoadCommand { get; }

    readonly List<IDisposable> subscriptions = new List<IDisposable>();
    readonly ILoadAssemblyService assemblyService;
    readonly ILoadAssemblyService _loadAssemblyService;
    readonly IRecentFileService _recentFileService;

    public MainWindowViewModel(
      ILoadAssemblyService assemblyService,
      ILoadAssemblyService loadAssemblyService,
      IFileService fileService,
      IControlTypeService controlTypeService,
      IControlService controlService,
      IRecentFileService recentFileService)
    {
      this.assemblyService = assemblyService;
      _loadAssemblyService = loadAssemblyService;
      _recentFileService = recentFileService;
      this.LoadCommand = ReactiveCommand.Create(Load);
      this.File = fileService.SelectedFile;

      this.subscriptions.Add(
        controlTypeService.ControlTypes.Connect()
          .Select(CreateControlViewModel)
          .Bind(out this._controls)
          .Subscribe()
      );

      this.subscriptions.Add(
        this.WhenAnyValue(vm => vm.SelectedControl)
          .Subscribe(vm => controlTypeService.SelectedControlType = vm?.ControlType));
      
      this.subscriptions.Add(
        controlTypeService.WhenAnyValue(s => s.SelectedControlType)
          .Subscribe(type => this.SelectedControl = this._controls.FirstOrDefault(vm => vm.ControlType == type))
        );

      this.subscriptions.Add(
         controlService.WhenAnyValue(s => s.Control)
           .Subscribe(c => this.Control = c));

      this.Control = null;
    }

    static ControlViewModel CreateControlViewModel(Type controlType)
      => new ControlViewModel { ControlType = controlType, Name = controlType.Name };

    void Load()
    {
      var file = this.File;
      
      _loadAssemblyService.File = file;
      _recentFileService.RecentFiles.Insert(0, file);
    }
  }
}