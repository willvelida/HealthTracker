using HealthTracker.Activity.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HealthTracker.Activity.Functions
{
    public class GetAllActivities
    {
        private readonly IActivityService _activityService;
        private readonly ILogger<GetAllActivities> _logger;

        public GetAllActivities(IActivityService activityService, ILogger<GetAllActivities> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        [FunctionName(nameof(GetAllActivities))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Activity")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                var activities = await _activityService.GetAllActivityRecords();

                if (activities is null)
                {
                    result = new NotFoundResult();
                    return result;
                }

                result = new OkObjectResult(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(GetAllActivities)}: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
