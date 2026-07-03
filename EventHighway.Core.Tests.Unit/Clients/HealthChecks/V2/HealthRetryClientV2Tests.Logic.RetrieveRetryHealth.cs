// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthRetryClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveRetryHealthV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            RetryHealthSummaryV2 randomSummary =
                CreateRandomRetryHealthSummaryV2();

            RetryHealthSummaryV2 returnedSummary =
                randomSummary;

            RetryHealthSummaryV2 expectedSummary =
                returnedSummary.DeepClone();

            this.retrySummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(randomCancellationToken))
                    .ReturnsAsync(returnedSummary);

            // when
            RetryHealthSummaryV2 actualSummary =
                await this.healthRetryClientV2
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            // then
            actualSummary.Should()
                .BeEquivalentTo(expectedSummary);

            this.retrySummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(randomCancellationToken),
                    Times.Once);

            this.retrySummaryV2OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
