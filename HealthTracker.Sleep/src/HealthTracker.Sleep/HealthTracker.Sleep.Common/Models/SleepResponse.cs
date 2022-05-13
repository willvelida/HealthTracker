using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Sleep.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class SleepResponse
    {
        public List<Sleep> sleep { get; set; }
        public Summary summary { get; set; }
    }
}
