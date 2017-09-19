using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Azure.KeyVault.Models;
using Xunit;

namespace DevOpsFlex.KeyVaultPocoCLI.Tests
{
    public class KeyVaultAccessTests
    {
        [Fact, Trait("Category", "Unit")]
        public void RunSemanticChecks_NoTags_Fail()
        {
            //arrange
            var secret = new SecretItem("https://rmtestkeyvault.vault.azure.net:443/secrets/Secret1", null, null);
            //act
            var ret = KeyValueAccess.RunSemanticChecks(new List<SecretItem>(new[] {secret}), "Name");
            //asert
            ret.ShouldBeEquivalentTo(false);
        }

        [Fact, Trait("Category", "Unit")]
        public void RunSemanticChecks_MissingTag_Fail()
        {
            //arrange
            var secret = new SecretItem("https://rmtestkeyvault.vault.azure.net:443/secrets/Secret1", null,
                new ConcurrentDictionary<string, string>(
                    new List<KeyValuePair<string, string>>(new[] {new KeyValuePair<string, string>("Blah", "Blah")})));
            //act
            var ret = KeyValueAccess.RunSemanticChecks(new List<SecretItem>(new[] { secret }), "Name");
            //asert
            ret.ShouldBeEquivalentTo(false);
        }

        [Fact, Trait("Category", "Unit")]
        public void RunSemanticChecks_Success()
        {
            //arrange
            var secret = new SecretItem("https://rmtestkeyvault.vault.azure.net:443/secrets/Secret1", null,
                new ConcurrentDictionary<string, string>(
                    new List<KeyValuePair<string, string>>(new[]
                    {
                        new KeyValuePair<string, string>("Name", "Blah"),
                        new KeyValuePair<string, string>("Type", "Blah")
                    })));
            //act
            var ret = KeyValueAccess.RunSemanticChecks(new List<SecretItem>(new[] { secret }), "Name", "Type");
            //asert
            ret.ShouldBeEquivalentTo(true);
        }
    }
}
