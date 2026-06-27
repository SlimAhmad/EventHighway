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
    public partial class HealthParticipantClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveParticipantSummaryV2Async()
        {
            // given . when
            Func<Task> retrieveParticipantSummaryV2Task = async () =>
            {
                IEnumerable<ParticipantSummaryV2> actualParticipantSummaryV2s =
                    await this.clientBroker.RetrieveParticipantSummaryV2Async(
                        TrafficPeriodV2.Day, DateTimeOffset.MinValue);

                actualParticipantSummaryV2s.Should().NotBeNull();
            };

            // then
            await retrieveParticipantSummaryV2Task.Should()
                .NotThrowAsync();
        }
    }
}
