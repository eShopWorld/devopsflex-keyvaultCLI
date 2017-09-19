using Microsoft.Extensions.CommandLineUtils;

namespace DevOpsFlex.KeyVaultPocoCLI
{
    public class CommandLineOptions
    {
        /// <summary>
        /// application id - credential to access the vault
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// application secret given to the application id - credential to access the vault
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// tenant identifier hosting the vault
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// name of the vault to open
        /// </summary>
        public string KeyVaultName { get; set; }
        /// <summary>
        /// name of the application to generate the POCO for 
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// optional namespace to use for generated POCOs
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// name of the tag to denote obsolete status
        /// defaulted to 'Obsolete'
        /// </summary>
        public string ObsoleteTagName { get; set; }
        /// <summary>
        /// name of the tag denoting type name assignation (class)
        /// defaulted to 'Type'
        /// </summary>
        public string TypeTagName { get; set; }
        /// <summary>
        /// name of the tag denoting name of the field (within  the class, <see cref="TypeTagName"/>
        /// defaulted to 'Name'
        /// </summary>
        public string NameTagName { get; set; }
        /// <summary>
        /// designates whether user has requested help for the command line options
        /// </summary>
        public bool IsHelp { get; set; }
        /// <summary>
        /// folder to output generated files into
        /// </summary>
        public string OutputFolder { get; set; }
        /// <summary>
        /// version number to inject into nuspec
        /// </summary>
        public string Version { get; set; }
        public static CommandLineOptions Parse(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = "dotnet kv-poco",
                FullName = "eShopWorld KeyVault c# tooling"
            };

            var options = new CommandLineOptions();

            options.Configure(app);

            app.Execute(args);
            options.IsHelp = app.IsShowingInformation;

            return options;
        }



        private void Configure(CommandLineApplication app)
        {
            var tenantIdOption = app.Option("--tenantId|-t", "tenant id of the Azure subscription",
                CommandOptionType.SingleValue);
            var appIdOption = app.Option("--appId|-aid", "application Id", CommandOptionType.SingleValue);
            var appSecretOption = app.Option("--appSecret|-as", "application secret", CommandOptionType.SingleValue);
            var keyVaultOption = app.Option("--keyVault|-k", "key vault name", CommandOptionType.SingleValue);
            var namespaceOption = app.Option("--namespace|-n", "optional namespace", CommandOptionType.SingleValue);
            var obsoleteTagOption = app.Option("--obsoleteTag|-ot", "name of the tag to denote obsolete status",
                CommandOptionType.SingleValue);
            var typeTagOption = app.Option("--typeTag|-tt", "name of the tag to denote class (type)",
                CommandOptionType.SingleValue);
            var nameTagOption = app.Option("--nameTag|-nt", "name of the tag to denote name of the field",
                CommandOptionType.SingleValue);
            var appNameOption =
                app.Option("--appName|-an", "application name to target", CommandOptionType.SingleValue);
            var outputFolderOption = app.Option("--output|-o", "output folder (default to current)", CommandOptionType.SingleValue);
            var versionOption = app.Option("--version|-v", "version number to use for the nuspec",
                CommandOptionType.SingleValue);

            app.HelpOption("--help|-h");

            app.OnExecute(() =>
            {
                AppId = appIdOption.Value();
                AppSecret = appSecretOption.Value();
                TenantId = tenantIdOption.Value();
                KeyVaultName = keyVaultOption.Value();
                Namespace = namespaceOption.Value();
                ObsoleteTagName = obsoleteTagOption.HasValue() ? obsoleteTagOption.Value() : "Obsolete";
                TypeTagName = typeTagOption.HasValue() ? typeTagOption.Value() : "Type";
                NameTagName = nameTagOption.HasValue() ? nameTagOption.Value() : "Name";
                AppName = appNameOption.Value();
                OutputFolder = outputFolderOption.HasValue()? outputFolderOption.Value() : ".";
                Version = versionOption.Value();

                return 0;
            });

            IsHelp = app.IsShowingInformation;
        }
    }
}
