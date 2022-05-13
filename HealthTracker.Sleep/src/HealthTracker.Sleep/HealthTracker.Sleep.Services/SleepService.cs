using AutoMapper;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.Interfaces;
using HealthTracker.Sleep.Services.Interfaces;
using Microsoft.Extensions.Logging;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Sleep.Services
{
    public class SleepService : ISleepService
    {
        private readonly IMapper _mapper;
        private readonly IServiceBusRepository _serviceBusRepository;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<SleepService> _logger;

        public SleepService(IMapper mapper, IServiceBusRepository serviceBusRepository, ILogger<SleepService> logger, ICosmosDbRepository cosmosDbRepository)
        {
            _mapper = mapper;
            _serviceBusRepository = serviceBusRepository;
            _logger = logger;
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task MapSleepEnvelopeAndSaveToDatabase(mdl.Sleep sleep)
        {
            try
            {
                if (sleep is null)
                    throw new Exception("No Sleep Document to map");

                mdl.SleepEnvelope sleepEnvelope = new mdl.SleepEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Sleep = sleep,
                    DocumentType = "Sleep",
                    Date = sleep.SleepDate
                };

                await _cosmosDbRepository.CreateSleepDocument(sleepEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(AddSleepDocument)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapAndSendSleepRecordToQueue(string date, SleepResponse sleepResponse)
        {
            try
            {
                var sleep = new mdl.Sleep();
                sleep.SleepDate = date;
                _mapper.Map(sleepResponse, sleep);

                await _serviceBusRepository.SendMessageToQueue(sleep);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSendSleepRecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
