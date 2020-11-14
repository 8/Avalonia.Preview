using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Fluent;
using Avalonia.Preview.ViewModels;
using ReactiveUI;
using static Avalonia.Fluent.GridEx;
using static Avalonia.Fluent.Brushes;
using static Avalonia.Fluent.Defaults.ColorPalettes;

namespace Avalonia.Preview.Views
{
  public class MainView : UserControl
  {
    public MainView()
    {
      this.content(
          new Grid()
            .rows(
              row().auto(),
              row().star()
            )
            .children(
              new SettingsView(),
              new Border()
                .row(1)
                .borderBrush(brush(Gray, 9))
                .bind(BackgroundProperty, "BackgroundBrush")
                .bind(BorderBrushProperty, "BorderBrush")
                .bindToDataContext(
                  Decorator.PaddingProperty,
                  (IMainWindowViewModel vm) => vm.WhenAnyValue(vm => vm.Padding)
                    .Select(value => new Thickness(value)))
                .bindToDataContext(
                  Decorator.MarginProperty,
                    (IMainWindowViewModel vm) => vm.WhenAnyValue(vm => vm.Margin)
                      .Select(value => new Thickness(value)))
                .bindToDataContext(
                    Border.BorderThicknessProperty,
                    (IMainWindowViewModel vm) => vm.WhenAnyValue(vm => vm.BorderThickness)
                      .Select(value => new Thickness(value)))
                .child(
                  new PreviewView()
                )
            )
        )
        ;
    }
  }
}