using HealthTracker.Common.Models;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HealthTracker.Sleep.Repository
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly Settings _settings;
        private readonly ILogger<CosmosDbRepository> _logger;

        public CosmosDbRepository(CosmosClient cosmosClient, ILogger<CosmosDbRepository> logger, IOptions<Settings> options)
        {
            _cosmosClient = cosmosClient;
            _logger = logger;
            _settings = options.Value;
            _container = _cosmosClient.GetContainer(_settings.DatabaseName, _settings.ContainerName);
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

        public async Task<List<SleepEnvelope>> GetAllSleepDocuments()
        {
            try
            {
                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM Records c WHERE c.DocumentType = 'Sleep'");
                QueryRequestOptions queryRequestOptions = new QueryRequestOptions { PartitionKey = new PartitionKey("DocumentType") };
                List<SleepEnvelope> sleepEnvelopes = new List<SleepEnvelope>();

                FeedIterator<SleepEnvelope> feedIterator = _container.GetItemQueryIterator<SleepEnvelope>(queryDefinition, null, queryRequestOptions);
                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<SleepEnvelope> queryResponse = await feedIterator.ReadNextAsync();
                    sleepEnvelopes.AddRange(queryResponse.Resource);
                }

                return sleepEnvelopes;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllSleepDocuments)}: {ex.Message}");
                throw;
            }
        }

        public async Task<SleepEnvelope> GetSleepEnvelopeByDate(string sleepDate)
        {
            try
            {
                QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM Records c WHERE c.DocumentType = 'Sleep' AND c.Sleep.SleepDate = @sleepDate")
                    .WithParameter("@sleepDate", sleepDate);
                QueryRequestOptions queryRequestOptions = new QueryRequestOptions { PartitionKey = new PartitionKey("DocumentType") };
                List<SleepEnvelope> sleepEnvelopes = new List<SleepEnvelope>();

                FeedIterator<SleepEnvelope> feedIterator = _container.GetItemQueryIterator<SleepEnvelope>(queryDefinition, null, queryRequestOptions);
                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<SleepEnvelope> queryResponse = await feedIterator.ReadNextAsync();
                    sleepEnvelopes.AddRange(queryResponse.Resource);
                }

                return sleepEnvelopes.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSleepEnvelopeByDate)}: {ex.Message}");
                throw;
            }
        }
    }
}
