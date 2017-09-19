using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.KeyVault.Models;
using Xunit;

namespace DevOpsFlex.KeyVaultPocoCLI.Tests
{    
    public class PocoGeneratorIntTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task GeneratePocoClass_Success()
        {
            //arrange
            var options = new CommandLineOptions() {AppName = "appName", NameTagName = "Name", ObsoleteTagName = "Obsolete", TypeTagName = "Type"};
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
            var content = await PocoGenerator.GeneratePoco(options, secrets);
            //assert            
            content.ShouldBeEquivalentTo(
                "using System;\r\n\r\nnamespace eShopWorld.appName\r\n{\r\n    public class ConfigurationSecrets\r\n    {\r\n\t\tpublic string Field1 {get; set;}\n\t\tpublic string Field2 {get; set;}\n\t\t[Obsolete]\n\t\tpublic string Field3 {get; set;}\n    }\r\n}");
        }

        [Fact, Trait("Category", "Integration")]
        public async Task GeneratePocoProject_Success()
        {
            //arrange
            var options = new CommandLineOptions() { AppName = "appName", Version = "1.1.1"};
            //act
            var content = await PocoGenerator.GenerateProject(options);
            //assert
            content.ShouldBeEquivalentTo("\r\n<Project Sdk=\"Microsoft.NET.Sdk\">\r\n\r\n  <PropertyGroup>\r\n    <TargetFramework>netstandard2.0</TargetFramework>\r\n    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>\r\n    <PackageRequireLicenseAcceptance>false</PackageRequireLicenseAcceptance>\r\n    <PackageId>eShopWorld.appName.ConfigurationSecrets</PackageId>\r\n    <Version>1.1.1</Version>\r\n    <Authors>eShopWorld</Authors>\r\n    <Company>eShopWorld</Company>\r\n    <Product>product</Product>\r\n    <Description>C# KeyVault representation for  appName</Description>\r\n    <Copyright>eShopWorld</Copyright>    \r\n    <AssemblyVersion>1.1.1</AssemblyVersion>\r\n  </PropertyGroup>\r\n</Project>");
        }
    }
}
