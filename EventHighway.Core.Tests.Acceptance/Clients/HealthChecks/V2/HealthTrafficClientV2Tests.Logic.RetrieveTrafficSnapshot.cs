// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.HealthChecks.V2
{
    public partial class HealthTrafficClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2Async()
        {
            // given . when
            Func<Task> retrieveTrafficSnapshotV2Task = async () =>
            {
                TrafficSnapshotV2 actualTrafficSnapshotV2 =
                    await this.clientBroker.RetrieveTrafficSnapshotV2Async(
                        TrafficPeriodV2.Day, DateTimeOffset.MinValue);

                actualTrafficSnapshotV2.Should().NotBeNull();
            };

            // then
            await retrieveTrafficSnapshotV2Task.Should()
                .NotThrowAsync();
        }
    }
}
