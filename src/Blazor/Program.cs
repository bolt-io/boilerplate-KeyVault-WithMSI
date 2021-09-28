using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();

                    var kvName = builtConfig["KeyVault:Name"];

                    if (kvName != null)
                    {

                        var kvPrefix = builtConfig["KeyVault:SecretsPrefix"];
                        var kvReloadIntervalInMinutes = builtConfig.GetValue<int>("KeyVault:ReloadIntervalInMinutes", default);

                        TimeSpan? kvReloadInterval = kvReloadIntervalInMinutes == default ? null : new TimeSpan(hours: 0, minutes: kvReloadIntervalInMinutes, seconds: 0);

                        var kvOptions = new AzureKeyVaultConfigurationOptions() { ReloadInterval = kvReloadInterval, Manager = new KeyVault.KeyVaultSecretPrefixManager(kvPrefix) };


                        config.AddAzureKeyVault(
                            vaultUri: new Uri($"https://{kvName}.vault.azure.net/"),
                            credential: new ChainedTokenCredential(new ManagedIdentityCredential()
                                                                        , new AzureCliCredential()
                            ), kvOptions);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
