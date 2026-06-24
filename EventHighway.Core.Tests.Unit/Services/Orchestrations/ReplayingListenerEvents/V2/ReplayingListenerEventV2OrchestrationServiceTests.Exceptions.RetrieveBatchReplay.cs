// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ReplayingListenerEvents.V2
{
    public partial class ReplayingListenerEventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptionsForRetrieveBatchReplay))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetrieveBatchReplayIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();

            var expectedReplayingListenerEventV2OrchestrationDependencyValidationException =
                new ReplayingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchTask =
                this.replayingListenerEventV2OrchestrationService
                    .RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken);

            ReplayingListenerEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationDependencyValidationException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingListenerEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptionsForRetrieveBatchReplay))]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveBatchReplayIfDependencyErrorOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();

            var expectedReplayingListenerEventV2OrchestrationDependencyException =
                new ReplayingListenerEventV2OrchestrationDependencyException(
                    message: "Replaying listener event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchTask =
                this.replayingListenerEventV2OrchestrationService
                    .RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken);

            ReplayingListenerEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationDependencyException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRetrieveBatchReplayIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            var serviceException = new Exception();

            var failedReplayingListenerEventV2OrchestrationServiceException =
                new FailedReplayingListenerEventV2OrchestrationServiceException(
                    message: "Failed replaying listener event orchestration service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedReplayingListenerEventV2OrchestrationServiceException =
                new ReplayingListenerEventV2OrchestrationServiceException(
                    message: "Replaying listener event service error occurred, contact support.",
                    innerException: failedReplayingListenerEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchTask =
                this.replayingListenerEventV2OrchestrationService
                    .RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken);

            ReplayingListenerEventV2OrchestrationServiceException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationServiceException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingListenerEventV2OrchestrationServiceException))),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveBatchReplayIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutReplayingListenerEventV2OrchestrationException =
                new TimeoutReplayingListenerEventV2OrchestrationException(
                    message: "Failed replaying listener event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedReplayingListenerEventV2OrchestrationDependencyException =
                new ReplayingListenerEventV2OrchestrationDependencyException(
                    message: "Replaying listener event dependency error occurred, contact support.",
                    innerException: timeoutReplayingListenerEventV2OrchestrationException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchTask =
                this.replayingListenerEventV2OrchestrationService
                    .RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken);

            ReplayingListenerEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationDependencyException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
