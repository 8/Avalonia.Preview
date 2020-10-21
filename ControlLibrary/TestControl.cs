using Avalonia.Controls;

namespace ControlLibrary
{
  public class TestControl : ContentControl
  {
    public TestControl()
    {
      this.Content = new TextBlock {Text = "This is a test."};
    }
  }

  public class MyLabel : TextBlock
  {
    public MyLabel()
    {
      this.Text = "This is my label.";
    }
  }
}