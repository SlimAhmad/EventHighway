// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Configurations;

namespace EventHighway.Core.Brokers.Configurations
{
    internal class ConfigurationBroker : IConfigurationBroker
    {
        private readonly EventHighwayConfiguration configuration;

        public ConfigurationBroker(EventHighwayConfiguration configuration) =>
            this.configuration = configuration;

        public EventHighwayConfiguration GetEventHighwayConfiguration() =>
            this.configuration;
    }
}
