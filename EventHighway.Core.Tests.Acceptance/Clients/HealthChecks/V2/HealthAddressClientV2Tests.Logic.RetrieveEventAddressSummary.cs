// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    public partial class HealthAddressClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveEventAddressSummaryV2Async()
        {
            // given . when
            Func<Task> retrieveEventAddressSummaryV2Task = async () =>
            {
                IEnumerable<EventAddressSummaryV2> actualSummaries =
                    await this.clientBroker.RetrieveEventAddressSummaryV2Async(
                        TrafficPeriodV2.Day, DateTimeOffset.MinValue);

                actualSummaries.Should().NotBeNull();
            };

            // then
            await retrieveEventAddressSummaryV2Task.Should()
                .NotThrowAsync();
        }
    }
}
