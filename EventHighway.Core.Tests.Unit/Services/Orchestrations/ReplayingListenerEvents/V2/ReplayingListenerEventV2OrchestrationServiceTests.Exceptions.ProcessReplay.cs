// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnProcessReplayIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListener.PromotedProperties = null;

            var expectedReplayingListenerEventV2OrchestrationDependencyValidationException =
                new ReplayingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<ListenerEventV2> processReplayTask =
                this.replayingListenerEventV2OrchestrationService
                    .ProcessReplayListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            ReplayingListenerEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationDependencyValidationException>(
                    processReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationDependencyValidationException);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnProcessReplayIfDependencyErrorOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListener.PromotedProperties = null;

            var expectedReplayingListenerEventV2OrchestrationDependencyException =
                new ReplayingListenerEventV2OrchestrationDependencyException(
                    message: "Replaying listener event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ThrowsAsync(dependencyException);

            // when
            ValueTask<ListenerEventV2> processReplayTask =
                this.replayingListenerEventV2OrchestrationService
                    .ProcessReplayListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            ReplayingListenerEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationDependencyException>(
                    processReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationDependencyException);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
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
            ShouldThrowServiceExceptionOnProcessReplayIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 someListenerEventV2 =
                CreateRandomListenerEventV2WithNavProps();

            someListenerEventV2.EventListener.PromotedProperties = null;

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

            this.eventCallV2ProcessingServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventCallV2 { IsSuccess = true });

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                .ReturnsAsync(GetRandomDateTimeOffset());

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken))
                .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV2> processReplayTask =
                this.replayingListenerEventV2OrchestrationService
                    .ProcessReplayListenerEventV2Async(
                        someListenerEventV2,
                        randomCancellationToken);

            ReplayingListenerEventV2OrchestrationServiceException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationServiceException>(
                    processReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationServiceException);

            this.eventCallV2ProcessingServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.ModifyListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    randomCancellationToken),
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
    }
}
