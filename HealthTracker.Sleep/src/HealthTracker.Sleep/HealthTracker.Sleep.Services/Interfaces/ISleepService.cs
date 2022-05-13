using mdl = HealthTracker.Common.Models;
using HealthTracker.Sleep.Common.Models;

namespace HealthTracker.Sleep.Services.Interfaces
{
    public interface ISleepService
    {
        Task MapAndSendSleepRecordToQueue(string date, SleepResponse sleepResponse);
        Task MapSleepEnvelopeAndSaveToDatabase(mdl.Sleep sleep);
    }
}
