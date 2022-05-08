using AutoMapper;
using HealthTracker.Activity.Common;
using HealthTracker.Activity.Repository.Interfaces;
using HealthTracker.Activity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IMapper _mapper;
        private readonly IServiceBusRepository _serviceBusRepository;
        private readonly ICosmosDbRepository _cosmosDbRepository;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(
            IMapper mapper,
            IServiceBusRepository serviceBusRepository,
            ILogger<ActivityService> logger, ICosmosDbRepository cosmosDbRepository)
        {
            _mapper = mapper;
            _serviceBusRepository = serviceBusRepository;
            _logger = logger;
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task MapActivityEnvelopeAndSaveToDatabase(mdl.Activity activity)
        {
            try
            {
                mdl.ActivityEnvelope activityEnvelope = new mdl.ActivityEnvelope
                {
                    Id = Guid.NewGuid().ToString(),
                    Activity = activity,
                    DocumentType = "Activity",
                    Date = activity.ActivityDate
                };

                await _cosmosDbRepository.CreateActivityDocument(activityEnvelope);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapActivityEnvelopeAndSaveToDatabase)}: {ex.Message}");
                throw;
            }
        }

        public async Task MapAndSendActivityRecordToQueue(string date, ActivityResponse activityResponse)
        {
            try
            {
                var activity = new mdl.Activity();
                activity.ActivityDate = date;
                _mapper.Map(activityResponse, activity);

                await _serviceBusRepository.SendMessageToQueue(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSendActivityRecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
