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
    public partial class HealthLoopClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveLoopDetectionSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            LoopDetectionSummaryV2 randomSummary =
                CreateRandomLoopDetectionSummaryV2();

            LoopDetectionSummaryV2 returnedSummary =
                randomSummary;

            LoopDetectionSummaryV2 expectedSummary =
                returnedSummary.DeepClone();

            this.loopDetectionV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedSummary);

            // when
            LoopDetectionSummaryV2 actualSummary =
                await this.healthLoopClientV2
                    .RetrieveLoopDetectionSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualSummary.Should()
                .BeEquivalentTo(expectedSummary);

            this.loopDetectionV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken),
                        Times.Once);

            this.loopDetectionV2OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
