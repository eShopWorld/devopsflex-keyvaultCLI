using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DevOpsFlex.KeyVaultPocoCLI
{
    public class GeneratePocoProjectCommand : AbstractRazorCommand<GeneratePocoProjectViewModel>
    {
        /// <inheritdoc/>
        public GeneratePocoProjectCommand(IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider):base(viewEngine, tempDataProvider, serviceProvider)
        {
                
        }

        public async Task<string> Run(CommandLineOptions options)
        {
            return await RunViewEngine("Views\\PocoProjectFile.cshtml",
                new GeneratePocoProjectViewModel {AppName = options.AppName, Version = options.Version});
        }
    }
}
