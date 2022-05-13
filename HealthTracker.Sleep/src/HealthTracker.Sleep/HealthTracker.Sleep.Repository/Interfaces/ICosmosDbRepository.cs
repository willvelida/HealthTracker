using HealthTracker.Common.Models;

namespace HealthTracker.Sleep.Repository.Interfaces
{
    public interface ICosmosDbRepository
    {
        Task CreateSleepDocument(SleepEnvelope sleepEnvelope);
        Task<List<SleepEnvelope>> GetAllSleepDocuments();
        Task<SleepEnvelope> GetSleepEnvelopeByDate(string sleepDate);
    }
}
