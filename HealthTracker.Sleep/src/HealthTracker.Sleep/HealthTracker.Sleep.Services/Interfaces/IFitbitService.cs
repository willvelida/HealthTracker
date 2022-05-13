using HealthTracker.Sleep.Common.Models;

namespace HealthTracker.Sleep.Services.Interfaces
{
    public interface IFitbitService
    {
        Task<SleepResponse> GetSleepResonse(string date);
    }
}
