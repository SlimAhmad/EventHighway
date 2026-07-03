// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.LoopDetections.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.LoopDetections.V2
{
    public partial class LoopDetectionV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveLoopDetectionSummaryAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveLoopDetectionSummaryTask =
                this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveLoopDetectionSummaryTask.AsTask);

            actualException.Should().NotBeOfType<LoopDetectionV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<LoopDetectionV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveLoopDetectionSummaryIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutLoopDetectionV2OrchestrationException =
                new TimeoutLoopDetectionV2OrchestrationException(
                    message: "Failed loop detection orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedLoopDetectionV2OrchestrationDependencyException =
                new LoopDetectionV2OrchestrationDependencyException(
                    message: "Loop detection dependency error occurred, contact support.",
                    innerException: timeoutLoopDetectionV2OrchestrationException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveLoopDetectionSummaryTask =
                this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            LoopDetectionV2OrchestrationDependencyException
                actualLoopDetectionV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<LoopDetectionV2OrchestrationDependencyException>(
                        retrieveLoopDetectionSummaryTask.AsTask);

            // then
            actualLoopDetectionV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedLoopDetectionV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLoopDetectionV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveLoopDetectionSummaryIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedLoopDetectionV2OrchestrationDependencyException =
                new LoopDetectionV2OrchestrationDependencyException(
                    message: "Loop detection dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveLoopDetectionSummaryTask =
                this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            LoopDetectionV2OrchestrationDependencyException
                actualLoopDetectionV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<LoopDetectionV2OrchestrationDependencyException>(
                        retrieveLoopDetectionSummaryTask.AsTask);

            // then
            actualLoopDetectionV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedLoopDetectionV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLoopDetectionV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveLoopDetectionSummaryIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedLoopDetectionV2OrchestrationServiceException =
                new FailedLoopDetectionV2OrchestrationServiceException(
                    message: "Failed loop detection service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedLoopDetectionV2OrchestrationServiceException =
                new LoopDetectionV2OrchestrationServiceException(
                    message: "Loop detection service error occurred, contact support.",
                    innerException: failedLoopDetectionV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveLoopDetectionSummaryTask =
                this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            LoopDetectionV2OrchestrationServiceException
                actualLoopDetectionV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<LoopDetectionV2OrchestrationServiceException>(
                        retrieveLoopDetectionSummaryTask.AsTask);

            // then
            actualLoopDetectionV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedLoopDetectionV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedLoopDetectionV2OrchestrationServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
