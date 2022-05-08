namespace HealthTracker.Common.Models
{
    public class Sleep
    {
        public string SleepDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MinutesAsleep { get; set; }
        public int MinutesAwake { get; set; }
        public int NumberOfAwakenings { get; set; }
        public int TimeInBed { get; set; }
        public int MinutesREMSleep { get; set; }
        public int MinutesLightSleep { get; set; }
        public int MinutesDeepSleep { get; set; }
    }
}
