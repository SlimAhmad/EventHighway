// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptionsForProcessReplayed))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnProcessReplayedIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();

            var expectedReplayingEventV2CoordinationDependencyValidationException =
                new ReplayingEventV2CoordinationDependencyValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(CreateBatchConfiguration(randomTake));

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2CoordinationService
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2CoordinationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyValidationException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2CoordinationDependencyValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingEventV2CoordinationDependencyValidationException))),
                Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptionsForProcessReplayed))]
        public async Task
            ShouldThrowDependencyExceptionOnProcessReplayedIfDependencyErrorOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();

            var expectedReplayingEventV2CoordinationDependencyException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(CreateBatchConfiguration(randomTake));

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(dependencyException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2CoordinationService
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2CoordinationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2CoordinationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingEventV2CoordinationDependencyException))),
                Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnProcessReplayedIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            var serviceException = new Exception();

            var failedReplayingEventV2CoordinationServiceException =
                new FailedReplayingEventV2CoordinationServiceException(
                    message: "Failed replaying event coordination service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedReplayingEventV2CoordinationServiceException =
                new ReplayingEventV2CoordinationServiceException(
                    message: "Replaying event service error occurred, contact support.",
                    innerException: failedReplayingEventV2CoordinationServiceException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(CreateBatchConfiguration(randomTake));

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(serviceException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2CoordinationService
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2CoordinationServiceException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationServiceException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2CoordinationServiceException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingEventV2CoordinationServiceException))),
                Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnProcessReplayedIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutReplayingEventV2CoordinationException =
                new TimeoutReplayingEventV2CoordinationException(
                    message: "Failed replaying event coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedReplayingEventV2CoordinationDependencyException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: timeoutReplayingEventV2CoordinationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(CreateBatchConfiguration(randomTake));

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(operationCanceledException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2CoordinationService
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2CoordinationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2CoordinationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingEventV2CoordinationDependencyException))),
                Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
