using Autofac;

namespace Avalonia.Preview
{
  public class AppModule : Autofac.Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterAssemblyTypes(ThisAssembly)
        .InNamespaceOf<ViewModels.ViewModelBase>()
        .AsImplementedInterfaces();

      builder.RegisterAssemblyTypes(ThisAssembly)
        .InNamespaceOf<Services.ControlService>()
        .AsImplementedInterfaces()
        .SingleInstance();
    }
  }
}