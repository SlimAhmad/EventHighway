// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveScheduledPendingIfEventV2DependencyAndLogItAsync(
            Xeption eventV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventV2OrchestrationDependencyException =
                new EventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV2DependencyException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventV2DependencyException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveScheduledPendingEventV2sTask =
                this.eventV2OrchestrationService.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken);

            EventV2OrchestrationDependencyException actualEventV2OrchestrationDependencyException =
                await Assert.ThrowsAsync<EventV2OrchestrationDependencyException>(
                    retrieveScheduledPendingEventV2sTask.AsTask);

            // then
            actualEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationDependencyException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveScheduledPendingIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV2OrchestrationServiceException =
                new FailedEventV2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2OrchestrationServiceException =
                new EventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2OrchestrationServiceException);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveScheduledPendingEventV2sTask =
                this.eventV2OrchestrationService.RetrieveScheduledPendingEventV2sAsync(
                    randomCancellationToken);

            EventV2OrchestrationServiceException actualEventV2OrchestrationServiceException =
                await Assert.ThrowsAsync<EventV2OrchestrationServiceException>(
                    retrieveScheduledPendingEventV2sTask.AsTask);

            // then
            actualEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationServiceException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationServiceException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveScheduledPendingIfTimeoutOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventV2OrchestrationException =
                new TimeoutEventV2OrchestrationException(
                    message: "Failed event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventV2OrchestrationDependencyException =
                new EventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2OrchestrationException);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveScheduledPendingEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveScheduledPendingEventV2sTask =
                this.eventV2OrchestrationService.RetrieveScheduledPendingEventV2sAsync(
                    TestContext.Current.CancellationToken);

            EventV2OrchestrationDependencyException actualEventV2OrchestrationDependencyException =
                await Assert.ThrowsAsync<EventV2OrchestrationDependencyException>(
                    retrieveScheduledPendingEventV2sTask.AsTask);

            // then
            actualEventV2OrchestrationDependencyException.Should().BeEquivalentTo(
                expectedEventV2OrchestrationDependencyException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveScheduledPendingEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveScheduledPendingAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IQueryable<EventV2>> retrieveScheduledPendingEventV2sTask =
                this.eventV2OrchestrationService.RetrieveScheduledPendingEventV2sAsync(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveScheduledPendingEventV2sTask.AsTask);

            actualException.Should().NotBeOfType<EventV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<EventV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
