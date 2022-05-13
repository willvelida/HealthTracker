using HealthTracker.Common.Models;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.Sleep.Repository
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly Settings _settings;
        private readonly ILogger<CosmosDbRepository> _logger;

        public CosmosDbRepository(CosmosClient cosmosClient, Container container, ILogger<CosmosDbRepository> logger, IOptions<Settings> options)
        {
            _cosmosClient = cosmosClient;
            _container = container;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task CreateSleepDocument(SleepEnvelope sleepEnvelope)
        {
            try
            {
                ItemRequestOptions itemRequestOptions = new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                };

                await _container.CreateItemAsync(sleepEnvelope, new PartitionKey(sleepEnvelope.DocumentType), itemRequestOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepDocument)}: {ex.Message}");
                throw;
            }
        }
    }
}
