using Avalonia.Controls;
using Avalonia.Fluent;
using Avalonia.Preview.ViewModels;
using Avalonia.Styling;
using ReactiveUI;
using static Avalonia.Fluent.GridEx;
using static Avalonia.Fluent.Styles;

namespace Avalonia.Preview.Views
{
  public class PreviewView : UserControl
  {
    public PreviewView()
    {
      this.styles(
        style()
          .selector(Selectors.Is<TextBlock>(null).Class("label"))
          .setters(setter(TextBlock.MarginProperty, new Thickness(4,2)))
      );
      
      this.Content =
        new Grid()
          .rows(
            row().star()
          )
          .children(
            new Border()
              .child(
                new ContentControl()
                  .bind(ContentControl.ContentProperty, "Control")
              )
          )
        ;
    }
  }
}