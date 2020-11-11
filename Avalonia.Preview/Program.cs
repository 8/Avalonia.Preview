using Avalonia.ReactiveUI;

namespace Avalonia.Preview
{
  static class EntryPoint
  {
    public static void Main(string[] args) => BuildAvaloniaApp()
      .StartWithClassicDesktopLifetime(args);

    static AppBuilder BuildAvaloniaApp()
      => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseReactiveUI();
  }
}