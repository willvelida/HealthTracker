using HealthTracker.Sleep.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HealthTracker.Sleep.Functions
{
    public class GetSleepRecordByDate
    {
        private readonly ISleepService _sleepService;
        private readonly ILogger<GetSleepRecordByDate> _logger;

        public GetSleepRecordByDate(ISleepService sleepService, ILogger<GetSleepRecordByDate> logger)
        {
            _sleepService = sleepService;
            _logger = logger;
        }

        [FunctionName(nameof(GetSleepRecordByDate))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Sleep/{sleepDate}")] HttpRequest req,
            string sleepDate)
        {
            IActionResult result;

            try
            {
                var sleepResponse = await _sleepService.GetSleepEnvelopeByDate(sleepDate);
                if (sleepResponse is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(sleepResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetSleepRecordByDate)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
