using System.Reactive.Linq;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Avalonia.Preview.Services;

namespace Avalonia.Preview.Test.Services
{
  public class LoadAssemblyServiceTest
  {
    readonly ITestOutputHelper output;

    public LoadAssemblyServiceTest(ITestOutputHelper output)
    {
      this.output = output;
    }
    
    LoadAssemblyService CreateService()
    {
      var mockFileWatcherService = new Mock<IFileWatcherService>();
      mockFileWatcherService.Setup(m => m.FileChanged).Returns(Observable.Return(@"c:\temp\testfile.dll"));
      
      return new LoadAssemblyService(
        mockFileWatcherService.Object
      );
    }
    
    [Fact]
    public void LoadAssemblyServiceTest_Ctor_Test()
    {
      CreateService();
    }

    [Fact]
    public void LoadAssemblyServiceTest_GetTargetFilePath_Test()
    {
      /* arrange */
      var service = CreateService();
      
      /* act */
      var sourcePath = service.GetTargetFilePath(
        @"C:\projects\Avalonia.Preview\ControlLibrary\bin\Debug\netcoreapp3.1\ControlLibrary.dll");

      this.output.Dump(sourcePath);
    }
  }
}