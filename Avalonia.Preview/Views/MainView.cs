using Avalonia.Controls;
using Avalonia.Fluent;
using Avalonia.Layout;
using Avalonia.Styling;

using static Avalonia.Fluent.GridEx;
using static Avalonia.Fluent.Styles;

namespace Avalonia.Preview.Views
{
  public class MainView : UserControl
  {
    public MainView()
    {
      this.styles(
        style()
          .selector(Selectors.Is<TextBlock>(null).Class("label"))
          .setters(
            setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Right),
            setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center),
            setter(TextBlock.PaddingProperty, new Thickness(4,2))
          )
        ,
        style()
          .selector(Selectors.Is<TextBox>(null).Class("input"))
          .setters(
            // setter(TextBox.PaddingProperty, new Thickness(4,2))
            setter(TextBox.MarginProperty, new Thickness(4,2))
          )
      );
      
      this.Content =
        new Grid()
          .columns(
            col().auto(),
            col().star(),
            col().auto()
          )
          .rows(
            row().auto(),
            row().auto(),
            row().star()
          )
          .children(
            new TextBlock()
              .text("File")
              .classes("label")
              .col(0)
            ,
            new TextBox()
              .bind(TextBox.TextProperty, "File")
              .classes("input")
              .col(1)
            ,
            new TextBlock()
              .text("Control Type")
              .classes("label")
              .row(1)
            ,
            new TextBox()
              .bind(TextBox.TextProperty, "ControlType")
              .classes("input")
              .row(1)
              .col(1)
          )
        ;
    }
  }
}
