// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnTargetedReplayIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            var expectedException =
                new ReplayingEventV2CoordinationDependencyValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventV2Id, eventAddressId, eventListenerIds, false, randomCancellationToken);

            ReplayingEventV2CoordinationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyValidationException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnTargetedReplayIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            var expectedException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventV2Id, eventAddressId, eventListenerIds, false, randomCancellationToken);

            ReplayingEventV2CoordinationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnTargetedReplayIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutReplayingEventV2CoordinationException =
                new TimeoutReplayingEventV2CoordinationException(
                    message: "Failed replaying event coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: timeoutReplayingEventV2CoordinationException);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventV2Id, eventAddressId, eventListenerIds, false,
                    TestContext.Current.CancellationToken);

            ReplayingEventV2CoordinationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnTargetedReplayIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            var serviceException = new Exception();

            var failedReplayingEventV2CoordinationServiceException =
                new FailedReplayingEventV2CoordinationServiceException(
                    message: "Failed replaying event coordination service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedException =
                new ReplayingEventV2CoordinationServiceException(
                    message: "Replaying event service error occurred, contact support.",
                    innerException: failedReplayingEventV2CoordinationServiceException);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventV2Id, eventAddressId, eventListenerIds, false, randomCancellationToken);

            ReplayingEventV2CoordinationServiceException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationServiceException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnTargetedReplayAsync()
        {
            // given
            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventV2Id, eventAddressId, eventListenerIds, false, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    replayTask.AsTask);

            actualException.Should().NotBeOfType<ReplayingEventV2CoordinationDependencyException>();
            actualException.Should().NotBeOfType<ReplayingEventV2CoordinationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
