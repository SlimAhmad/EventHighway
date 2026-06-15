// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.Core.Models.Configurations.Healths
{
    public class HealthConfiguration
    {
        public List<RagThreshold> Thresholds { get; set; } = new List<RagThreshold>
        {
            new RagThreshold { Metric = HealthMetric.DeadEvents,    Green = 0,    Red = 6  },
            new RagThreshold { Metric = HealthMetric.ErrorRate,     Green = 9.99m, Red = 25 },
            new RagThreshold { Metric = HealthMetric.HandlerCount,  Green = 1,    Red = 0  },
        };
    }
}
