﻿using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Preview.ViewModels;
using Avalonia.Preview.Views;

namespace Avalonia.Preview
{
  public class App : Application
  {
    ILifetimeScope Container;
    
    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      this.Container = new ContainerFactory().CreateContainer();
      
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
        desktop.MainWindow = new MainWindow
        {
          DataContext = this.Container.Resolve<IMainWindowViewModel>(),
        };
      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
