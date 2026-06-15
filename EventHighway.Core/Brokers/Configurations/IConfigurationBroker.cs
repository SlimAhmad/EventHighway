// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations;

namespace EventHighway.Core.Brokers.Configurations
{
    internal interface IConfigurationBroker
    {
        EventHighwayConfiguration GetEventHighwayConfiguration();
    }
}
