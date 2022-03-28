using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using HealthTracker.RefreshToken;
using HealthTracker.RefreshToken.Common.Settings;
using HealthTracker.RefreshToken.Repository;
using HealthTracker.RefreshToken.Repository.Interfaces;
using HealthTracker.RefreshToken.Services;
using HealthTracker.RefreshToken.Services.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace HealthTracker.RefreshToken
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddHttpClient();
            builder.Services.AddLogging();

            builder.Services.AddOptions<Settings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Settings").Bind(settings);
                });

            builder.Services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
                SecretClientOptions secretClientOptions = new SecretClientOptions()
                {
                    Retry =
                    {
                        Delay = TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                return new SecretClient(new Uri(configuration["KeyVaultUri"]), new DefaultAzureCredential(), secretClientOptions);
            });

            builder.Services.AddTransient<IKeyVaultRepository, KeyVaultRepository>();
            builder.Services.AddTransient<IKeyVaultService, KeyVaultService>();
            builder.Services.AddHttpClient<IRefreshTokenService, RefreshTokenService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(15))
                .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }
    }
}
