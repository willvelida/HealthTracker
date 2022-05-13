using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Sleep.Common.Models
{
    [ExcludeFromCodeCoverage]
    public class Stages
    {
        public int deep { get; set; }
        public int light { get; set; }
        public int rem { get; set; }
        public int wake { get; set; }
    }
}
