using System;
using System.Diagnostics;
using System.Reactive;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Avalonia.Preview.ViewModels;
using Avalonia.Preview.Views;
using ReactiveUI;

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

      RxApp.DefaultExceptionHandler = Observer.Create<Exception>(
        ex => Debug.WriteLine(ex),
        ex => Debug.WriteLine(ex)
      );

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
