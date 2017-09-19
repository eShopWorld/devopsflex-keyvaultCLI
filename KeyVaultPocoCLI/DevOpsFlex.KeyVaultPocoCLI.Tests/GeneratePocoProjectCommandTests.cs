using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace DevOpsFlex.KeyVaultPocoCLI.Tests
{

    public class GeneratePocoProjectCommandTests
    {
        [Fact, Trait("Category", "Unit")]
        public async Task Run_Success()
        {
            //arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockRazorViewEngine = new Mock<IRazorViewEngine>();

            var mockView = new Mock<IView>();

            mockRazorViewEngine.Setup(i => i.GetView(It.IsAny<string>(), It.Is<string>(s => s == "Views\\PocoProjectFile.cshtml"), It.IsAny<bool>()))
                .Returns(ViewEngineResult.Found("Views\\PocoProjectFile.cshtml", mockView.Object)).Verifiable();

            mockView.Setup(i => i.RenderAsync(It.IsAny<ViewContext>())).Returns(Task.CompletedTask).Verifiable();

            var sut = new GeneratePocoProjectCommand(mockRazorViewEngine.Object, mockTempDataProvider.Object, mockServiceProvider.Object);
            //act
            await sut.Run(new CommandLineOptions());
            //assert
            mockRazorViewEngine.VerifyAll();
            mockView.VerifyAll();
        }
        
    }
}
