// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.DuplicateSummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.DuplicateSummaries.V2
{
    public partial class DuplicateSummaryV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveDuplicateDetectionSummaryAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveDuplicateDetectionSummaryTask =
                this.duplicateSummaryV2OrchestrationService
                    .RetrieveDuplicateDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveDuplicateDetectionSummaryTask.AsTask);

            actualException.Should().NotBeOfType<DuplicateSummaryV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<DuplicateSummaryV2OrchestrationServiceException>();
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
        public async Task ShouldThrowDependencyExceptionOnRetrieveDuplicateDetectionSummaryIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutDuplicateSummaryV2OrchestrationException =
                new TimeoutDuplicateSummaryV2OrchestrationException(
                    message: "Failed duplicate summary orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedDuplicateSummaryV2OrchestrationDependencyException =
                new DuplicateSummaryV2OrchestrationDependencyException(
                    message: "Duplicate summary dependency error occurred, contact support.",
                    innerException: timeoutDuplicateSummaryV2OrchestrationException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveDuplicateDetectionSummaryTask =
                this.duplicateSummaryV2OrchestrationService
                    .RetrieveDuplicateDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            DuplicateSummaryV2OrchestrationDependencyException
                actualDuplicateSummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<DuplicateSummaryV2OrchestrationDependencyException>(
                        retrieveDuplicateDetectionSummaryTask.AsTask);

            // then
            actualDuplicateSummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedDuplicateSummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDuplicateSummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDuplicateDetectionSummaryIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedDuplicateSummaryV2OrchestrationDependencyException =
                new DuplicateSummaryV2OrchestrationDependencyException(
                    message: "Duplicate summary dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveDuplicateDetectionSummaryTask =
                this.duplicateSummaryV2OrchestrationService
                    .RetrieveDuplicateDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            DuplicateSummaryV2OrchestrationDependencyException
                actualDuplicateSummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<DuplicateSummaryV2OrchestrationDependencyException>(
                        retrieveDuplicateDetectionSummaryTask.AsTask);

            // then
            actualDuplicateSummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedDuplicateSummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDuplicateSummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveDuplicateDetectionSummaryIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedDuplicateSummaryV2OrchestrationServiceException =
                new FailedDuplicateSummaryV2OrchestrationServiceException(
                    message: "Failed duplicate summary service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedDuplicateSummaryV2OrchestrationServiceException =
                new DuplicateSummaryV2OrchestrationServiceException(
                    message: "Duplicate summary service error occurred, contact support.",
                    innerException: failedDuplicateSummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveDuplicateDetectionSummaryTask =
                this.duplicateSummaryV2OrchestrationService
                    .RetrieveDuplicateDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            DuplicateSummaryV2OrchestrationServiceException
                actualDuplicateSummaryV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<DuplicateSummaryV2OrchestrationServiceException>(
                        retrieveDuplicateDetectionSummaryTask.AsTask);

            // then
            actualDuplicateSummaryV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedDuplicateSummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedDuplicateSummaryV2OrchestrationServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
