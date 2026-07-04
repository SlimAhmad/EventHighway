// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Configurations.Retries;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Unit.Brokers.Configurations
{
    public partial class ConfigurationBrokerTests
    {
        [Fact]
        public void ShouldGetRetryConfiguration()
        {
            // given
            RetryConfiguration randomRetryConfiguration = CreateRandomRetryConfiguration();
            RetryConfiguration expectedRetryConfiguration = randomRetryConfiguration.DeepClone();

            EventHighwayConfiguration eventHighwayConfiguration =
                new EventHighwayConfiguration
                {
                    RetryConfiguration = randomRetryConfiguration
                };

            var configurationBroker =
                new ConfigurationBroker(eventHighwayConfiguration);

            // when
            RetryConfiguration actualRetryConfiguration =
                configurationBroker.GetRetryConfiguration();

            // then
            actualRetryConfiguration.Should().BeEquivalentTo(expectedRetryConfiguration);
        }
    }
}
