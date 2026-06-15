// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.Healths;

namespace EventHighway.Core.Brokers.Configurations
{
    internal interface IConfigurationBroker
    {
        HealthConfiguration GetHealthConfiguration();
        BatchConfiguration GetBatchConfiguration();
    }
}
