// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthTrafficClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            TrafficSnapshotV2 randomTrafficSnapshotV2 =
                CreateRandomTrafficSnapshotV2();

            TrafficSnapshotV2 returnedTrafficSnapshotV2 =
                randomTrafficSnapshotV2;

            TrafficSnapshotV2 expectedTrafficSnapshotV2 =
                returnedTrafficSnapshotV2.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedTrafficSnapshotV2);

            // when
            TrafficSnapshotV2 actualTrafficSnapshotV2 =
                await this.healthTrafficClientV2
                    .RetrieveTrafficSnapshotV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualTrafficSnapshotV2.Should()
                .BeEquivalentTo(expectedTrafficSnapshotV2);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
