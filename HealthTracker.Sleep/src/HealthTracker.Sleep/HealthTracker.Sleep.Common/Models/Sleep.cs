using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Sleep.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class Sleep
    {
        public int awakeCount { get; set; }
        public int awakeDuration { get; set; }
        public int awakeningsCount { get; set; }
        public string dateOfSleep { get; set; }
        public int duration { get; set; }
        public int efficiency { get; set; }
        public DateTime endTime { get; set; }
        public bool isMainSleep { get; set; }
        public long logId { get; set; }
        public List<MinuteData> minuteData { get; set; }
        public int minutesAfterWakeup { get; set; }
        public int minutesAsleep { get; set; }
        public int minutesAwake { get; set; }
        public int minutesToFallAsleep { get; set; }
        public int restlessCount { get; set; }
        public int restlessDuration { get; set; }
        public DateTime startTime { get; set; }
        public int timeInBed { get; set; }
    }
}
