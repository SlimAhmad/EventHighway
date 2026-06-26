// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    public partial class HealthLoopClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveLoopDetectionSummaryV2Async()
        {
            // given . when
            Func<Task> retrieveLoopDetectionSummaryV2Task = async () =>
            {
                LoopDetectionSummaryV2 actualSummary =
                    await this.clientBroker.RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, DateTimeOffset.MinValue);

                actualSummary.Should().NotBeNull();
            };

            // then
            await retrieveLoopDetectionSummaryV2Task.Should()
                .NotThrowAsync();
        }
    }
}
