using HealthTracker.Sleep.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HealthTracker.Sleep.Functions
{
    public class GenerateSleepLogByDate
    {
        private readonly IFitbitService _fitbitService;
        private readonly ISleepService _sleepService;
        private readonly ILogger<GenerateSleepLogByDate> _logger;

        public GenerateSleepLogByDate(IFitbitService fitbitService, ISleepService sleepService, ILogger<GenerateSleepLogByDate> logger)
        {
            _fitbitService = fitbitService;
            _sleepService = sleepService;
            _logger = logger;
        }

        [FunctionName(nameof(GenerateSleepLogByDate))]
        public async Task Run([TimerTrigger("0 15 5 * * *")] TimerInfo myTimer)
        {
            try
            {
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                _logger.LogInformation($"Attempting to retrieve Sleep log for {dateParameter}");
                var sleepResponse = await _fitbitService.GetSleepResonse(dateParameter);

                _logger.LogInformation($"Mapping response to Sleep Object and sending to queue");
                await _sleepService.MapAndSendSleepRecordToQueue(dateParameter, sleepResponse);
                _logger.LogInformation($"Sleep Summary sent to queue");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GenerateSleepLogByDate)}: {ex.Message}");
                throw;
            }
        }
    }
}
