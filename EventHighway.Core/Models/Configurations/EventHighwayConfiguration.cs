// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Healths;

namespace EventHighway.Core.Models.Configurations
{
    public class EventHighwayConfiguration
    {
        public HealthConfiguration Health { get; set; } = new HealthConfiguration();
        public BatchConfiguration BatchProcessing { get; set; } = new BatchConfiguration();
    }
}
