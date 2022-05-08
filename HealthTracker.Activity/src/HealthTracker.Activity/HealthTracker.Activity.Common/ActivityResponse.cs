using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Activity.Common
{
    [ExcludeFromCodeCoverage]
    public class ActivityResponse
    {
        public List<object> activities { get; set; }
        public Goals goals { get; set; }
        public Summary summary { get; set; }
    }
}
