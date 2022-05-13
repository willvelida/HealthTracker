using HealthTracker.Sleep.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Sleep.Functions
{
    public class CreateSleepRecord
    {
        private readonly ISleepService _sleepService;
        private readonly ILogger<CreateSleepRecord> _logger;

        public CreateSleepRecord(ISleepService sleepService, ILogger<CreateSleepRecord> logger)
        {
            _sleepService = sleepService;
            _logger = logger;
        }

        [FunctionName(nameof(CreateSleepRecord))]
        public async Task Run([ServiceBusTrigger("sleepqueue", Connection = "ServiceBusConnection")] string sleepQueueItem)
        {
            try
            {
                var sleep = JsonConvert.DeserializeObject<mdl.Sleep>(sleepQueueItem);
                await _sleepService.MapSleepEnvelopeAndSaveToDatabase(sleep);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(CreateSleepRecord)}: {ex.Message}");
                throw;
            }
        }
    }
}
