using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using DynamicData;
using DynamicData.Alias;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Preview.Services;

namespace Avalonia.Preview.ViewModels
{
  public interface IMainWindowViewModel
  {
    int Padding { get; set; } 
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

    int _padding;
    public int Padding
    {
      get => this._padding;
      set => this.RaiseAndSetIfChanged(ref _padding, value);
    }

    bool _isAlwaysOnTop;
    public bool IsAlwaysOnTop
    {
      get => this._isAlwaysOnTop;
      set => this.RaiseAndSetIfChanged(ref _isAlwaysOnTop, value);
    }

    public string[] BackgroundColors { get; }

    string _selectedBackgroundColor;
    public string SelectedBackgroundColor
    {
      get => _selectedBackgroundColor;
      set => this.RaiseAndSetIfChanged(ref _selectedBackgroundColor, value);
    }

    ReadOnlyObservableCollection<string> _themes;
    public ReadOnlyObservableCollection<string> Themes
    {
      get => _themes;
      set => this.RaiseAndSetIfChanged(ref _themes, value);
    }

    string _selectedTheme;
    public string SelectedTheme
    {
      get => this._selectedTheme;
      set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
    }

    SolidColorBrush _backgroundBrush;
    public SolidColorBrush BackgroundBrush
    {
      get => _backgroundBrush;
      set => this.RaiseAndSetIfChanged(ref _backgroundBrush, value);
    }

    public ICommand LoadCommand { get; }

    readonly List<IDisposable> subscriptions = new List<IDisposable>();
    readonly ILoadAssemblyService _loadAssemblyService;
    readonly IRecentFileService _recentFileService;

    public MainWindowViewModel(
      ILoadAssemblyService loadAssemblyService,
      IFileService fileService,
      IControlTypeService controlTypeService,
      IControlService controlService,
      IRecentFileService recentFileService,
      IThemeService themeService,
      IColorService colorService)
    {
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

      this.subscriptions.Add(
        themeService.Themes.Connect().Bind(out _themes).Subscribe()
      );
      
      this.subscriptions.Add(
        themeService.WhenAnyValue(s => s.SelectedTheme).Subscribe(theme => this.SelectedTheme = theme)
      );
      
      this.subscriptions.Add(
        this.WhenAnyValue(vm => vm.SelectedTheme).Subscribe(theme => this.SelectedTheme = theme)
      );

      this.BackgroundColors = colorService.Colors;
      this.SelectedBackgroundColor = this.BackgroundColors.FirstOrDefault();

      this.subscriptions.Add(
    this.WhenAnyValue(vm => vm.SelectedBackgroundColor)
        .Select(colorService.GetColorFromName)
        .Select(color => new SolidColorBrush(color ?? Colors.Transparent, 1))
        .Subscribe(brush => this.BackgroundBrush = brush)
      );

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