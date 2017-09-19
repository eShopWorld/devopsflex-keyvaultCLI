using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;

namespace DevOpsFlex.KeyVaultPocoCLI
{
    /// <summary>
    /// coordinator to generate Poco class as well as the project
    /// </summary>
    public static class PocoGenerator
    {
        /// <summary>
        /// generate Poco class
        /// </summary>
        /// <param name="options">command line options</param>
        /// <param name="secrets">secrets extracted from the keyvault</param>
        /// <returns>content of the Poco class</returns>
        public static async Task<string> GeneratePoco(CommandLineOptions options, IList<SecretItem> secrets)
        {
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            var provider = services.BuildServiceProvider();
            var serviceScope = provider.GetRequiredService<IServiceScopeFactory>();
            using (serviceScope.CreateScope())
            {   
                                
                var pocoCommand = provider.GetRequiredService<GeneratePocoClassCommand>();
                return await pocoCommand.Run(options, secrets);
            }
        }

        /// <summary>
        /// generate Poco project file
        /// </summary>
        /// <param name="options">command line options</param>
        /// <returns>content of the Poco project file</returns>
        public static async Task<string> GenerateProject(CommandLineOptions options)
        {
            var services = new ServiceCollection();
            ConfigureDefaultServices(services, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            var provider = services.BuildServiceProvider();
            var serviceScope = provider.GetRequiredService<IServiceScopeFactory>();
            using (serviceScope.CreateScope())
            {

                var pocoCommand = provider.GetRequiredService<GeneratePocoProjectCommand>();
                return await pocoCommand.Run(options);
            }
        }

        /// <summary>
        /// inject necessary services into the service collection - initialise the DI
        /// </summary>
        /// <param name="services">service collection</param>
        /// <param name="customApplicationBasePath">base path of the application runtime</param>
        public static void ConfigureDefaultServices(IServiceCollection services, string customApplicationBasePath)
        {
            var applicationEnvironment = PlatformServices.Default.Application;

            services.AddSingleton(applicationEnvironment);
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            IFileProvider fileProvider;
            string applicationName;
            if (!string.IsNullOrEmpty(customApplicationBasePath))
            {
                applicationName = Assembly.GetEntryAssembly().GetName().Name;
                fileProvider = new PhysicalFileProvider(customApplicationBasePath);
            }
            else
            {
                applicationName = Assembly.GetEntryAssembly().GetName().Name;
                fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            }

            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                ApplicationName = applicationName,
                WebRootFileProvider = fileProvider,
            });

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(fileProvider);
            });

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddTransient<GeneratePocoClassCommand>();
            services.AddTransient<GeneratePocoProjectCommand>();
        }
    }
}
