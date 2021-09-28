using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;

namespace Blazor.KeyVault
{
    public class KeyVaultSecretPrefixManager : KeyVaultSecretManager
    {
        private readonly string _prefix;


        public KeyVaultSecretPrefixManager(string? prefix)
        {
            prefix ??= "";
            _prefix = prefix.EndsWith("--") ? prefix : $"{prefix}--";
        }

        public override bool Load(SecretProperties secret)
        {
            return secret.Name.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase);
        }

        public override string GetKey(KeyVaultSecret secret)
        {
            return secret.Name
                .Substring(_prefix.Length)
                .Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}
