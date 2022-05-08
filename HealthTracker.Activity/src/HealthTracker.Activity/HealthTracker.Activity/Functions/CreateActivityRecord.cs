using HealthTracker.Activity.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Activity.Functions
{
    public class CreateActivityRecord
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<CreateActivityRecord> _logger;

        public CreateActivityRecord(IActivityService activityService, ILogger<CreateActivityRecord> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [FunctionName(nameof(CreateActivityRecord))]
        public async Task Run([ServiceBusTrigger("activityqueue", Connection = "ServiceBusConnection")] string activityQueueItem)
        {
            try
            {
                var activity = JsonConvert.DeserializeObject<mdl.Activity>(activityQueueItem);
                await _activityService.MapActivityEnvelopeAndSaveToDatabase(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateActivityRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}