// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllEventArchivesIfTimeoutOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutEventArchiveV2OrchestrationException =
                new TimeoutEventArchiveV2OrchestrationException(
                    message: "Failed event archive orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: timeoutEventArchiveV2OrchestrationException);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2OrchestrationService.RetrieveAllEventArchiveV2sAsync(
                    TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetrieveAllEventArchivesIfDependencyValidationExceptionOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<IQueryable<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2OrchestrationService.RetrieveAllEventArchiveV2sAsync(
                    randomCancellationToken);

            EventArchiveV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyValidationException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyValidationException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveAllEventArchivesIfDependencyExceptionOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2OrchestrationService.RetrieveAllEventArchiveV2sAsync(
                    randomCancellationToken);

            EventArchiveV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllEventArchivesAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IQueryable<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2OrchestrationService.RetrieveAllEventArchiveV2sAsync(
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllTask.AsTask);

            actualException.Should().NotBeOfType<EventArchiveV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<EventArchiveV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
