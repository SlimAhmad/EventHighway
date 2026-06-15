// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Configurations.Healths
{
    public class RagThreshold
    {
        public HealthMetric Metric { get; set; }
        public decimal Green { get; set; }
        public decimal Red { get; set; }
    }
}
