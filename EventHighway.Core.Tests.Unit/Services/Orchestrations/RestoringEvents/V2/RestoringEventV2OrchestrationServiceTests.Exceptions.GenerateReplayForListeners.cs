// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RestoringEvents.V2
{
    public partial class RestoringEventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnGenerateReplayForListenersIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            List<Guid> someEventListenerIds = new List<Guid> { GetRandomId() };

            var expectedException =
                new RestoringEventV2OrchestrationDependencyValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask generateReplayTask =
                this.restoringEventV2OrchestrationService.GenerateReplayForListenersAsync(
                    someEventArchiveV2s,
                    someEventListenerIds,
                    randomCancellationToken);

            RestoringEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationDependencyValidationException>(
                    generateReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnGenerateReplayForListenersIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            List<Guid> someEventListenerIds = new List<Guid> { GetRandomId() };

            var expectedException =
                new RestoringEventV2OrchestrationDependencyException(
                    message: "Restoring event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask generateReplayTask =
                this.restoringEventV2OrchestrationService.GenerateReplayForListenersAsync(
                    someEventArchiveV2s,
                    someEventListenerIds,
                    randomCancellationToken);

            RestoringEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationDependencyException>(
                    generateReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnGenerateReplayForListenersIfTimeoutOccursAndLogItAsync()
        {
            // given
            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            List<Guid> someEventListenerIds = new List<Guid> { GetRandomId() };

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutRestoringEventV2OrchestrationException =
                new TimeoutRestoringEventV2OrchestrationException(
                    message: "Failed restoring event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedException =
                new RestoringEventV2OrchestrationDependencyException(
                    message: "Restoring event dependency error occurred, contact support.",
                    innerException: timeoutRestoringEventV2OrchestrationException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask generateReplayTask =
                this.restoringEventV2OrchestrationService.GenerateReplayForListenersAsync(
                    someEventArchiveV2s,
                    someEventListenerIds,
                    TestContext.Current.CancellationToken);

            RestoringEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationDependencyException>(
                    generateReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnGenerateReplayForListenersIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            List<Guid> someEventListenerIds = new List<Guid> { GetRandomId() };

            var serviceException = new Exception();

            var failedRestoringEventV2OrchestrationServiceException =
                new FailedRestoringEventV2OrchestrationServiceException(
                    message: "Failed restoring event orchestration service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedException =
                new RestoringEventV2OrchestrationServiceException(
                    message: "Restoring event service error occurred, contact support.",
                    innerException: failedRestoringEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask generateReplayTask =
                this.restoringEventV2OrchestrationService.GenerateReplayForListenersAsync(
                    someEventArchiveV2s,
                    someEventListenerIds,
                    randomCancellationToken);

            RestoringEventV2OrchestrationServiceException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationServiceException>(
                    generateReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnGenerateReplayForListenersAsync()
        {
            // given
            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            List<Guid> someEventListenerIds = new List<Guid> { GetRandomId() };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask generateReplayTask =
                this.restoringEventV2OrchestrationService.GenerateReplayForListenersAsync(
                    someEventArchiveV2s,
                    someEventListenerIds,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    generateReplayTask.AsTask);

            actualException.Should().NotBeOfType<RestoringEventV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<RestoringEventV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
