﻿using HealthTracker.Activity.Common;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        Task MapAndSendActivityRecordToQueue(string date, ActivityResponse activityResponse);
        Task MapActivityEnvelopeAndSaveToDatabase(mdl.Activity activity);
    }
}
