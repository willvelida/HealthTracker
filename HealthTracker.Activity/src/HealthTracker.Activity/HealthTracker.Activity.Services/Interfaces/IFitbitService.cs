using HealthTracker.Activity.Common;

namespace HealthTracker.Activity.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<ActivityResponse> GetActivityResponse(string date);
    }
}
