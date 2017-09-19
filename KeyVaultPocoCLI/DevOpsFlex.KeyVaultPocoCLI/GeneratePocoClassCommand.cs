using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Azure.KeyVault.Models;

namespace DevOpsFlex.KeyVaultPocoCLI
{
    /// <inheritdoc />
    /// <summary>
    /// specific "command" to generate poco class that has recognized secrets as fields and is bindable to keyvault configuration extensions
    /// </summary>
    public class GeneratePocoClassCommand : AbstractRazorCommand<GeneratePocoClassViewModel>
    {
      
        /// <inheritdoc />      
        public GeneratePocoClassCommand(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider):base(viewEngine, tempDataProvider, serviceProvider)
        {
        }

        /// <summary>
        /// run the command i.e. generate the poco class as driven by secrets and their tags
        /// </summary>
        /// <param name="options">command line options</param>
        /// <param name="secrets">secrets contained in the keyvault</param>
        /// <returns></returns>
        public async Task<string> Run(CommandLineOptions options, IList<SecretItem> secrets)
        {            
            var viewModel = CreateViewModel(options.AppName, options.NameTagName, options.ObsoleteTagName, secrets);
            return await RunViewEngine("Views\\PocoClass.cshtml", viewModel);
        }

        /// <summary>
        /// create view model for ther underlying razor engine
        /// </summary>
        /// <param name="appName">name of the target app</param>
        /// <param name="nameTagName">name of the tag holding the property name</param>
        /// <param name="obsoleteTagName">name of the tag holding the obsolete flag</param>
        /// <param name="secrets">collection of secrets to process</param>
        /// <returns>view model instance</returns>

        public virtual GeneratePocoClassViewModel CreateViewModel(string appName, string nameTagName, string obsoleteTagName, IList<SecretItem> secrets)
        {
            return new GeneratePocoClassViewModel
            {
                Namespace = appName,
                Fields = secrets.Select(i => new Tuple<string, bool>(
                    i.Tags!=null && i.Tags.ContainsKey(nameTagName) ? i.Tags[nameTagName] : i.Identifier.Name,
                    i.Tags != null && i.Tags.ContainsKey(obsoleteTagName) && Convert.ToBoolean(i.Tags[obsoleteTagName])))
            };
        }
    }
}
