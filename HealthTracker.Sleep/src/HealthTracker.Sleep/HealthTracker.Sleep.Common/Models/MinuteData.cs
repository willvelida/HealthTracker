using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Sleep.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class MinuteData
    {
        public string dateTime { get; set; }
        public string value { get; set; }
    }
}
