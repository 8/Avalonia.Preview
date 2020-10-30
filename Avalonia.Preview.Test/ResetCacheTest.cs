using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Avalonia.Preview.Test
{
  public class ResetCacheTest
  {
    readonly ITestOutputHelper _output;

    public ResetCacheTest(ITestOutputHelper output)
    {
      _output = output;
    }

    [Fact]
    public void ResetCacheTest_AssemblyName_Test()
    {
      var assembly = Assembly.Load("DynamicData");

      this._output.Dump(assembly.FullName);
    }
    
    [Fact]
    public void ResetCacheTest_ResetCache_Test()
    {
      var assembly = Assembly.Load("DynamicData");

      assembly.Should().NotBeNull();

      var types = assembly.GetTypes();
      
      // foreach (var type in types)
      //   if (type.FullName.Contains("DynamicData.Binding.ObservablePropertyFactoryCache"))
      //     this._testOutputHelper.Dump(type.FullName);

      /* get the internal type */
      var type = Assembly.Load("DynamicData").GetTypes()
        .FirstOrDefault(t => t.FullName == "DynamicData.Binding.ObservablePropertyFactoryCache");
      
      /* get static instance */
      var instance = type.GetField("Instance").GetValue(null);

      instance.Should().NotBeNull();

      // this._output.Dump(instance.GetType());

      string fieldName = "_factories";

      // foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
      //   this._output.Dump(fieldInfo);

      var factoriesFieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
      factoriesFieldInfo.Should().NotBeNull();

      var dictionary = (ConcurrentDictionary<string, object>) factoriesFieldInfo.GetValue(instance);
      dictionary.Clear();

      // var type = Type.GetType("DynamicData.Binding.ObservablePropertyFactoryCache");

      // type.Should().NotBeNull();
    }
  
    // "static DynamicData.Binding.ObservablePropertyFactoryCache.Instance"
    // private readonly ConcurrentDictionary<string,object> _factories = new ConcurrentDictionary<string, object>();
    
  }

}