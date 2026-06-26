// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    public partial class HealthDuplicateClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveDuplicateDetectionSummaryV2Async()
        {
            // given . when
            Func<Task> retrieveDuplicateDetectionSummaryV2Task = async () =>
            {
                DuplicateDetectionSummaryV2 actualSummary =
                    await this.clientBroker.RetrieveDuplicateDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, DateTimeOffset.MinValue);

                actualSummary.Should().NotBeNull();
            };

            // then
            await retrieveDuplicateDetectionSummaryV2Task.Should()
                .NotThrowAsync();
        }
    }
}
