using System;
using Autofac;
using FluentAssertions;
using Xunit;
using Avalonia.Preview.Services;
using Avalonia.Preview.ViewModels;

namespace Avalonia.Preview.Test
{
  public class ContainerFactoryTest
  {
    ContainerFactory CreateFactory() => new ContainerFactory();
    
    [Fact]
    public void ContainerFactoryTest_Ctor_Test() => CreateFactory();
    
    [Theory,
      InlineData(typeof(IMainWindowViewModel)),
      InlineData(typeof(IControlService)),
      InlineData(typeof(ILoadAssemblyService)),
      InlineData(typeof(IControlTypeService)),
      InlineData(typeof(IFileService)),
      InlineData(typeof(IRecentFileService)),
      InlineData(typeof(IFileWatcherService)),
    ]
    public void ContainerFactoryTest_CreateContainer_Test(Type type)
    {
      using var container = CreateFactory().CreateContainer();
      var instance = container.Resolve(type);
      instance.Should().NotBeNull();
    }
  }
}