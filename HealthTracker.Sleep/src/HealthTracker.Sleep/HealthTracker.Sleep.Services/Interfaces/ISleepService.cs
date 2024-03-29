﻿using HealthTracker.Sleep.Common.Models;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Sleep.Services.Interfaces
{
    public interface ISleepService
    {
        Task MapAndSendSleepRecordToQueue(string date, SleepResponse sleepResponse);
        Task MapSleepEnvelopeAndSaveToDatabase(mdl.Sleep sleep);
        Task<List<mdl.SleepEnvelope>> GetAllSleepEnvelopeRecords();
        Task<mdl.SleepEnvelope> GetSleepEnvelopeByDate(string date);
    }
}
