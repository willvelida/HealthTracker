using HealthTracker.Activity.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HealthTracker.Activity.Functions
{
    public class GetActivitySummaryByDate
    {
        private readonly IFitbitService _fitbitService;
        private readonly IActivityService _activityService;
        private readonly ILogger<GetActivitySummaryByDate> _logger;

        public GetActivitySummaryByDate(
            IFitbitService fitbitService,
            IActivityService activityService,
            ILogger<GetActivitySummaryByDate> logger)
        {
            _fitbitService = fitbitService;
            _activityService = activityService;
            _logger = logger;
        }

        [FunctionName(nameof(GetActivitySummaryByDate))]
        public async Task Run([TimerTrigger("0 15 5 * * *")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"{nameof(GetActivitySummaryByDate)} executed at: {DateTime.Now}");
                var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                _logger.LogInformation($"Attempting to retrieve Daily Activity Summary for {date}");
                var activityResponse = await _fitbitService.GetActivityResponse(date);

                _logger.LogInformation($"Mapping response to Activity object and Sending to queue.");
                await _activityService.MapAndSendActivityRecordToQueue(date, activityResponse);
                _logger.LogInformation($"Activity Summary sent to queue.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivitySummaryByDate)}: {ex.Message}");
                throw;
            }
        }
    }
}
