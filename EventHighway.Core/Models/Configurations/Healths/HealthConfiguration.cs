// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.Core.Models.Configurations.Healths
{
    /// <summary>
    /// Configures the Red/Amber/Green health thresholds used by the EventHighway health-check system.
    /// Each <see cref="RagThreshold"/> in <see cref="Thresholds"/> controls one <see cref="HealthMetric"/>.
    /// A metric with no matching threshold is reported as <c>NA</c>.
    /// </summary>
    public class HealthConfiguration
    {
        /// <summary>
        /// Gets or sets the list of RAG thresholds applied during health evaluation.
        /// Defaults to thresholds for <see cref="HealthMetric.DeadEvents"/>,
        /// <see cref="HealthMetric.ErrorRate"/>, and <see cref="HealthMetric.HandlerCount"/>.
        /// Remove an entry to suppress RAG scoring for that metric.
        /// </summary>
        public List<RagThreshold> Thresholds { get; set; } = new List<RagThreshold>
        {
            new RagThreshold { Metric = HealthMetric.DeadEvents,    Green = 0,    Red = 6  },
            new RagThreshold { Metric = HealthMetric.ErrorRate,     Green = 9.99m, Red = 25 },
            new RagThreshold { Metric = HealthMetric.HandlerCount,  Green = 1,    Red = 0  },
            new RagThreshold { Metric = HealthMetric.LoopsDetected, Green = 0,    Red = 6  },
            new RagThreshold { Metric = HealthMetric.PendingBacklog,     Green = 50,    Red = 500 },
            new RagThreshold { Metric = HealthMetric.ReplayRate,         Green = 5,     Red = 20  },
            new RagThreshold { Metric = HealthMetric.ArchiveErrorRate,   Green = 9.99m, Red = 25  },
            new RagThreshold { Metric = HealthMetric.DeadArchivedEvents, Green = 0,     Red = 6   },
        };
    }
}
