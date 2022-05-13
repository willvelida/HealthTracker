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
    public class GetAllSleepRecords
    {
        private readonly ISleepService _sleepService;
        private readonly ILogger<GetAllSleepRecords> _logger;

        public GetAllSleepRecords(ISleepService sleepService, ILogger<GetAllSleepRecords> logger)
        {
            _sleepService = sleepService;
            _logger = logger;
        }

        [FunctionName(nameof(GetAllSleepRecords))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Sleep")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                var sleepRecords = await _sleepService.GetAllSleepEnvelopeRecords();
                if (sleepRecords is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(sleepRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllSleepRecords)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
