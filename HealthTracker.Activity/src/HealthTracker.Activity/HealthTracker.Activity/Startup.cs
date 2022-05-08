using AutoMapper;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using HealthTracker.Activity;
using HealthTracker.Activity.Common;
using HealthTracker.Activity.Repository;
using HealthTracker.Activity.Repository.Interfaces;
using HealthTracker.Activity.Services;
using HealthTracker.Activity.Services.Interfaces;
using HealthTracker.Activity.Services.Mappers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace HealthTracker.Activity
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddLogging();

            builder.Services.AddOptions<Settings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Settings").Bind(settings);
                });

            builder.Services.AddAutoMapper(typeof(Startup));
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapActivityResponseToActivity());
            });
            var mapper = mappingConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            builder.Services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                SecretClientOptions secretClientOptions = new SecretClientOptions
                {
                    Retry =
                    {
                        Delay = TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(10),
                        MaxRetries = 5,
                        Mode = Azure.Core.RetryMode.Exponential
                    }
                };
                return new SecretClient(new Uri(configuration["KeyVaultUri"]), new DefaultAzureCredential(), secretClientOptions);
            });
            builder.Services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                ServiceBusClientOptions serviceBusClientOptions = new ServiceBusClientOptions
                {
                    RetryOptions =
                    {
                        Delay = TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(10),
                        MaxRetries = 5,
                        Mode = ServiceBusRetryMode.Exponential
                    }
                };
                return new ServiceBusClient(configuration["ServiceBusConnection"], new DefaultAzureCredential(), serviceBusClientOptions);
            });
            builder.Services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
                {
                    MaxRetryAttemptsOnRateLimitedRequests = 5,
                    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(10),
                };
                return new CosmosClient(configuration["CosmosDbEndpoint"], new DefaultAzureCredential(), cosmosClientOptions);
            });
            builder.Services.AddTransient<IKeyVaultRepository, KeyVaultRepository>();
            builder.Services.AddTransient<IServiceBusRepository, ServiceBusRepository>();
            builder.Services.AddTransient<ICosmosDbRepository, CosmosDbRepository>();
            builder.Services.AddTransient<IActivityService, ActivityService>();
            builder.Services.AddHttpClient<IFitbitService, FitbitService>()
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
