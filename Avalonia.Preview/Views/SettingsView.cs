using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Fluent;
using Avalonia.Layout;
using Avalonia.Styling;
using static Avalonia.Fluent.GridEx;
using static Avalonia.Fluent.Styles;

namespace Avalonia.Preview.Views
{
  public class SettingsView : UserControl
  {
    public SettingsView()
    {
      this.styles(
        style()
          .selector(Selectors.Is<TextBlock>(null).Class("label"))
          .setters(
            setter(HorizontalAlignmentProperty, HorizontalAlignment.Right),
            setter(VerticalAlignmentProperty, VerticalAlignment.Center),
            setter(TextBlock.PaddingProperty, new Thickness(4, 2))
          )
        ,
        style()
          .selector(Selectors.Is<TextBox>(null).Class("input"))
          .setters(
            setter(MarginProperty, new Thickness(4, 2))
          ),
        style()
          .selector(Selectors.Is<Button>(null).Class("button"))
          .setters(
            setter(MinWidthProperty, Ranges.value(7)),
            setter(MarginProperty, new Thickness(2, 2))
          ),
        style()
          .selector(Selectors.Is<ComboBox>(null).Class("combo"))
          .setters(
            setter(MarginProperty, new Thickness(4, 2)),
            setter(ComboBox.MinHeightProperty, 26)
          ),
        style()
          .selector(Selectors.Is<Slider>(null).Class("slider"))
          .setters(
            setter(VerticalAlignmentProperty, VerticalAlignment.Center)
          ),
        style()
          .selector(Selectors.Is<CheckBox>(null).Class("checkbox"))
          .setters(
            setter(MarginProperty, new Thickness(4, 2))
          ),
        style()
          .selector(Selectors.Is<NumericUpDown>(null))
          .setters(
            setter(MarginProperty, new Thickness(4, 2))
          )
      );
      
      this.Content =
        new Grid()
          .columns(
            col().auto(),
            col().star(),
            col().auto(),
            col().auto()
          )
          .rows(
            row().auto(),
            row().auto(),
            row().auto(),
            row().auto(),
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
            new Button()
              .bindCommand("LoadCommand")
              .classes("button")
              .col(3)
              .content("Load")
            ,
            new Button()
              .bindCommand("BrowseCommand")
              .classes("button")
              .col(2)
              .content("Browse")
            ,
            new TextBlock()
              .text("Control Type")
              .classes("label")
              .row(1)
            ,
            new ComboBox()
              .bind(ItemsControl.ItemsProperty, "Controls")
              .bind(SelectingItemsControl.SelectedItemProperty, "SelectedControl")
              .classes("combo")
              .row(1)
              .col(1)
            ,
            new TextBlock()
              .text("Margin")
              .classes("label")
              .row(2)
            ,
            new NumericUpDown()
              .bind(NumericUpDown.ValueProperty, "Margin")
              .row(2)
              .col(1)
              .minimum(0)
              .maximum(100)
            ,
            new Slider()
              .classes("slider")
              .bind(RangeBase.ValueProperty, "Margin")
              .minimum(0)
              .maximum(100)
              .row(2)
              .col(2)
              .colSpan(2)
            ,
            new TextBlock()
              .classes("label")
              .row(3)
              .text("Always on Top")
            ,
            new CheckBox()
              .row(3)
              .col(1)
              .classes("checkbox")
              .bind(ToggleButton.IsCheckedProperty, "IsAlwaysOnTop")
            ,
            new TextBlock()
              .classes("label")
              .row(4)
              .text("Background")
            ,
            new ComboBox()
              .col(1)
              .row(4)
              .bind(ItemsControl.ItemsProperty, "BackgroundColors")
              .bind(SelectingItemsControl.SelectedItemProperty, "SelectedBackgroundColor")
              .classes("combo")
            ,
            new TextBlock()
              .classes("label")
              .row(5)
              .text("Theme")
            ,
            new ComboBox()
              .col(1)
              .row(5)
              .classes("combo")
              .bind(ItemsControl.ItemsProperty, "Themes")
              .bind(SelectingItemsControl.SelectedItemProperty, "SelectedTheme")
          )
        ;
    }
  }
}