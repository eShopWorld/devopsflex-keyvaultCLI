using System;
using System.IO;
using System.Threading.Tasks;

namespace DevOpsFlex.KeyVaultPocoCLI
{
    public class Runner
    {
        static void Main(string[] args)
        {
            try
            {
                var options = CommandLineOptions.Parse(args);
                if (!options.IsHelp)
                    RunCommand(options).GetAwaiter().GetResult();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task RunCommand(CommandLineOptions options)
        {
            //collect all secrets
            var secrets = await KeyValueAccess.GetAllSecrets(options);
            Directory.CreateDirectory(options.OutputFolder);
            //generate class
            var content = await PocoGenerator.GeneratePoco(options, secrets);            
            await File.WriteAllTextAsync(Path.Combine(options.OutputFolder, "ConfigurationSecrets.cs"), content);
            //generate project file (with nuget spec)
            content = await PocoGenerator.GenerateProject(options);
            await File.WriteAllTextAsync(Path.Combine(options.OutputFolder, $"{options.AppName}.csproj"), content);
        }
    }
}
