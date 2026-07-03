// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
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
        public async Task ShouldThrowDependencyValidationExceptionOnRestoreIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            var expectedException =
                new RestoringEventV2OrchestrationDependencyValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask restoreTask =
                this.restoringEventV2OrchestrationService.RestoreAsync(
                    someEventArchiveV2s,
                    someListenerEventArchiveV2s,
                    randomCancellationToken);

            RestoringEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationDependencyValidationException>(
                    restoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
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
        public async Task ShouldThrowDependencyExceptionOnRestoreIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            var expectedException =
                new RestoringEventV2OrchestrationDependencyException(
                    message: "Restoring event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask restoreTask =
                this.restoringEventV2OrchestrationService.RestoreAsync(
                    someEventArchiveV2s,
                    someListenerEventArchiveV2s,
                    randomCancellationToken);

            RestoringEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationDependencyException>(
                    restoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
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
        public async Task ShouldThrowDependencyExceptionOnRestoreIfTimeoutOccursAndLogItAsync()
        {
            // given
            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

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

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask restoreTask =
                this.restoringEventV2OrchestrationService.RestoreAsync(
                    someEventArchiveV2s,
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            RestoringEventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationDependencyException>(
                    restoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
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
        public async Task ShouldThrowServiceExceptionOnRestoreIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

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

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask restoreTask =
                this.restoringEventV2OrchestrationService.RestoreAsync(
                    someEventArchiveV2s,
                    someListenerEventArchiveV2s,
                    randomCancellationToken);

            RestoringEventV2OrchestrationServiceException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationServiceException>(
                    restoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
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
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRestoreAsync()
        {
            // given
            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask restoreTask =
                this.restoringEventV2OrchestrationService.RestoreAsync(
                    someEventArchiveV2s,
                    someListenerEventArchiveV2s,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    restoreTask.AsTask);

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
