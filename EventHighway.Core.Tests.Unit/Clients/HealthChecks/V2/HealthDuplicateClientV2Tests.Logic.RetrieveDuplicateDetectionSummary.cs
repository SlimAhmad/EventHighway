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
    public partial class HealthDuplicateClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveDuplicateDetectionSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            DuplicateDetectionSummaryV2 randomSummary =
                CreateRandomDuplicateDetectionSummaryV2();

            DuplicateDetectionSummaryV2 returnedSummary =
                randomSummary;

            DuplicateDetectionSummaryV2 expectedSummary =
                returnedSummary.DeepClone();

            this.duplicateSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedSummary);

            // when
            DuplicateDetectionSummaryV2 actualSummary =
                await this.healthDuplicateClientV2
                    .RetrieveDuplicateDetectionSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualSummary.Should()
                .BeEquivalentTo(expectedSummary);

            this.duplicateSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken),
                        Times.Once);

            this.duplicateSummaryV2OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
