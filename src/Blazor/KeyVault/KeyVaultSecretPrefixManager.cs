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
            // Ensure prefix ends with "--" unless whitespace or empty or you'll not be able to access secrets without prefix
            if (!string.IsNullOrWhiteSpace(prefix) && !prefix.EndsWith("--"))
                prefix += "--";
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
