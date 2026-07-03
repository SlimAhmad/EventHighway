// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetrySummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RetrySummaries.V2
{
    public partial class RetrySummaryV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveRetryHealthAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<RetryHealthSummaryV2> retrieveRetryHealthTask =
                this.retrySummaryV2OrchestrationService
                    .RetrieveRetryHealthV2Async(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveRetryHealthTask.AsTask);

            actualException.Should().NotBeOfType<RetrySummaryV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<RetrySummaryV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveRetryHealthIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutRetrySummaryV2OrchestrationException =
                new TimeoutRetrySummaryV2OrchestrationException(
                    message: "Failed retry summary orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedRetrySummaryV2OrchestrationDependencyException =
                new RetrySummaryV2OrchestrationDependencyException(
                    message: "Retry summary dependency error occurred, contact support.",
                    innerException: timeoutRetrySummaryV2OrchestrationException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveRetryHealthTask =
                this.retrySummaryV2OrchestrationService
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            RetrySummaryV2OrchestrationDependencyException
                actualRetrySummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<RetrySummaryV2OrchestrationDependencyException>(
                        retrieveRetryHealthTask.AsTask);

            // then
            actualRetrySummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedRetrySummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetrySummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveRetryHealthIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedRetrySummaryV2OrchestrationDependencyException =
                new RetrySummaryV2OrchestrationDependencyException(
                    message: "Retry summary dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveRetryHealthTask =
                this.retrySummaryV2OrchestrationService
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            RetrySummaryV2OrchestrationDependencyException
                actualRetrySummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<RetrySummaryV2OrchestrationDependencyException>(
                        retrieveRetryHealthTask.AsTask);

            // then
            actualRetrySummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedRetrySummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetrySummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveRetryHealthIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedRetrySummaryV2OrchestrationServiceException =
                new FailedRetrySummaryV2OrchestrationServiceException(
                    message: "Failed retry summary service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedRetrySummaryV2OrchestrationServiceException =
                new RetrySummaryV2OrchestrationServiceException(
                    message: "Retry summary service error occurred, contact support.",
                    innerException: failedRetrySummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveRetryHealthTask =
                this.retrySummaryV2OrchestrationService
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            RetrySummaryV2OrchestrationServiceException
                actualRetrySummaryV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<RetrySummaryV2OrchestrationServiceException>(
                        retrieveRetryHealthTask.AsTask);

            // then
            actualRetrySummaryV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedRetrySummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRetrySummaryV2OrchestrationServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
