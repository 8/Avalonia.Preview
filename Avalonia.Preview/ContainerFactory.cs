using Autofac;

namespace Avalonia.Preview
{
  public class ContainerFactory
  {
    public ILifetimeScope CreateContainer()
    {
      var builder = new ContainerBuilder();
      builder.RegisterModule<AppModule>();
      return builder.Build();
    }
  }
}