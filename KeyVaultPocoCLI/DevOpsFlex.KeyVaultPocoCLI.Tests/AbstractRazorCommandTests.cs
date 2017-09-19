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
    public class AbstractRazorCommandTests
    {
        [Fact, Trait("Category", "Unit")]
        public async Task RunViewEngine_Success()
        {
            //arrange            
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockRazorViewEngine = new Mock<IRazorViewEngine>();

            var mockView = new Mock<IView>();

            mockRazorViewEngine.Setup(i => i.GetView(It.IsAny<string>(), It.Is<string>(s => s == "test"), It.IsAny<bool>()))
                .Returns(ViewEngineResult.Found("test", mockView.Object)).Verifiable();

            mockView.Setup(i => i.RenderAsync(It.IsAny<ViewContext>())).Returns(Task.CompletedTask).Verifiable();
            var sut = new TestCommand(mockRazorViewEngine.Object, mockTempDataProvider.Object,
                mockServiceProvider.Object);

            //act
            await sut.RunViewEngine("test", new TestViewModel());

            //assert
            mockView.VerifyAll();
            mockRazorViewEngine.VerifyAll();
        }

        /// <summary>
        /// test wrapper to provide testability to what is otherwise an abstract class
        /// </summary>
        public class TestCommand : AbstractRazorCommand<TestViewModel>
        {
            public TestCommand(
                IRazorViewEngine viewEngine,
                ITempDataProvider tempDataProvider,
                IServiceProvider serviceProvider):base(viewEngine, tempDataProvider, serviceProvider)
            {               
            }
        }

        public class TestViewModel
        {
            
        }
    }
}
