﻿using System.Diagnostics.CodeAnalysis;

namespace HealthTracker.Activity.Common
{
    [ExcludeFromCodeCoverage]
    public class HeartRateZone
    {
        public double caloriesOut { get; set; }
        public int max { get; set; }
        public int min { get; set; }
        public int minutes { get; set; }
        public string name { get; set; }
    }
}
