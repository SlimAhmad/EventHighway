// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Healths;

namespace EventHighway.Core.Models.Configurations
{
    /// <summary>
    /// Root configuration object passed to the EventHighway client at construction time.
    /// All sections default to safe, out-of-the-box values so the object is optional.
    /// </summary>
    public class EventHighwayConfiguration
    {
        /// <summary>
        /// Gets or sets the health-check configuration, including RAG thresholds for each metric.
        /// Defaults to a <see cref="HealthConfiguration"/> with standard thresholds pre-populated.
        /// </summary>
        public HealthConfiguration Health { get; set; } = new HealthConfiguration();

        /// <summary>
        /// Gets or sets the batch processing configuration, including the bulk-operation batch size.
        /// Defaults to a <see cref="BatchConfiguration"/> with system defaults.
        /// </summary>
        public BatchConfiguration BatchProcessing { get; set; } = new BatchConfiguration();
    }
}
