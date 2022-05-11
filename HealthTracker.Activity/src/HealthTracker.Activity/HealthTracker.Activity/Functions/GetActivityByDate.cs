using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HealthTracker.Activity.Services.Interfaces;

namespace HealthTracker.Activity.Functions
{
    public class GetActivityByDate
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<GetActivityByDate> _logger;

        public GetActivityByDate(IActivityService activityService, ILogger<GetActivityByDate> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [FunctionName(nameof(GetActivityByDate))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Activity/{activityDate}")] HttpRequest req,
            string activityDate)
        {
            IActionResult result;

            try
            {
                var activityResponse = await _activityService.GetActivityRecordByDate(activityDate);
                if (activityResponse == null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(activityResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetActivityByDate)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
