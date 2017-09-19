using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Azure.KeyVault.Models;
using Moq;
using Xunit;

namespace DevOpsFlex.KeyVaultPocoCLI.Tests
{
    public class GeneratePocoClassCommandTests
    {
        [Fact, Trait("Category", "Unit")]
        public async Task Run_Success()
        {
            //arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockRazorViewEngine = new Mock<IRazorViewEngine>();

            var mockView = new Mock<IView>();

            mockRazorViewEngine.Setup(i => i.GetView(It.IsAny<string>(), It.Is<string>(s => s == "Views\\PocoClass.cshtml"), It.IsAny<bool>()))
                .Returns(ViewEngineResult.Found("Views\\PocoClass.cshtml", mockView.Object)).Verifiable();

            mockView.Setup(i => i.RenderAsync(It.IsAny<ViewContext>())).Returns(Task.CompletedTask).Verifiable();

            var sut = new GeneratePocoClassCommand(mockRazorViewEngine.Object, mockTempDataProvider.Object, mockServiceProvider.Object);
            //act
            await sut.Run(new CommandLineOptions(), new List<SecretItem>());
            //assert
            mockRazorViewEngine.VerifyAll();
            mockView.VerifyAll();
        }

        [Fact, Trait("Category", "Unit")]
        public void CreateViewModel_Success()
        {
            //arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockRazorViewEngine = new Mock<IRazorViewEngine>();

            var sut = new GeneratePocoClassCommand(mockRazorViewEngine.Object, mockTempDataProvider.Object, mockServiceProvider.Object);
            var secrets = new List<SecretItem>(new[]
            {
                new SecretItem
                {
                    Id = "https://rmtestkeyvault.vault.azure.net:443/secrets/Secret1",
                    Tags = new Dictionary<string, string>(new List<KeyValuePair<string, string>>(
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("Type", "appName"),
                            new KeyValuePair<string, string>("Name", "Field1")
                        }))
                },
                new SecretItem
                {
                    Id = "https://rmtestkeyvault.vault.azure.net:443/secrets/Secret2",
                    Tags = new Dictionary<string, string>(new List<KeyValuePair<string, string>>(
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("Type", "diffApp"),
                            new KeyValuePair<string, string>("Name", "Field2")
                        }))
                },
                new SecretItem
                {
                    Id = "https://rmtestkeyvault.vault.azure.net:443/secrets/Secret3",                    
                    Tags = new Dictionary<string, string>(new List<KeyValuePair<string, string>>(
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("Type", "appName"),
                            new KeyValuePair<string, string>("Name", "Field3"),
                            new KeyValuePair<string, string>("Obsolete", "true")
                        }))
                }
            });

            //act
            var model = sut.CreateViewModel("appName", "Name", "Obsolete", secrets);
            //assert
            model.Namespace.ShouldBeEquivalentTo("appName");
            model.Fields.Count().ShouldBeEquivalentTo(3);
            model.Fields.Should()
                .Contain(new List<Tuple<string, bool>>(new[]
                    {
                        new Tuple<string, bool>("Field1", false),
                        new Tuple<string, bool>("Field2", false),
                        new Tuple<string, bool>("Field3", true)
                    }),
                    "because it was passed as an input");
        }
    }
}
