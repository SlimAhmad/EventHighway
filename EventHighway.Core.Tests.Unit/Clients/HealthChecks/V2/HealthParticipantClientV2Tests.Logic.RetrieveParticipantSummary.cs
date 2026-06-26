// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthParticipantClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveParticipantSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            IEnumerable<ParticipantSummaryV2> randomParticipantSummaryV2s =
                CreateRandomParticipantSummaryV2s();

            IEnumerable<ParticipantSummaryV2> returnedParticipantSummaryV2s =
                randomParticipantSummaryV2s;

            IEnumerable<ParticipantSummaryV2> expectedParticipantSummaryV2s =
                returnedParticipantSummaryV2s.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveParticipantSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedParticipantSummaryV2s);

            // when
            IEnumerable<ParticipantSummaryV2> actualParticipantSummaryV2s =
                await this.healthParticipantClientV2
                    .RetrieveParticipantSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualParticipantSummaryV2s.Should()
                .BeEquivalentTo(expectedParticipantSummaryV2s);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveParticipantSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
