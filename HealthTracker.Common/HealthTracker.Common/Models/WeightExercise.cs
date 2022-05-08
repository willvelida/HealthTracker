namespace HealthTracker.Common.Models
{
    public class WeightExercise
    {
        public string WeightExerciseId { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public string Notes { get; set; }
    }
}
