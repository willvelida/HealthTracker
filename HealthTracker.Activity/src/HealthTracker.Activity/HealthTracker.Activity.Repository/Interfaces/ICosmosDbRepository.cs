using HealthTracker.Common.Models;

namespace HealthTracker.Activity.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateActivityDocument(ActivityEnvelope activityEnvelope);
        Task<List<ActivityEnvelope>> GetActivities();
        Task<ActivityEnvelope> GetActivityByDate(string activityDate);
    }
}
