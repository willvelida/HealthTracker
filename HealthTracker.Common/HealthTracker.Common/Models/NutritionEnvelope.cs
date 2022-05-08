using Newtonsoft.Json;

namespace HealthTracker.Common.Models
{
    public class NutritionEnvelope
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Nutrition Nutrition { get; set; }
        public string Date { get; set; }
        public string DocumentType { get; set; }
    }
}
