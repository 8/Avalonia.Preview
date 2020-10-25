using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Preview.ViewModels;
using Avalonia.Preview.Views;

namespace Avalonia.Preview
{
  public class App : Application
  {
    ILifetimeScope _container;
    
    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      this._container = new ContainerFactory().CreateContainer();
      
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
        var window =  new MainWindow
        {
          DataContext = this._container.Resolve<IMainWindowViewModel>(),
        };
        
        #if DEBUG
        window.AttachDevTools();
        #endif

        desktop.MainWindow = window;
      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
