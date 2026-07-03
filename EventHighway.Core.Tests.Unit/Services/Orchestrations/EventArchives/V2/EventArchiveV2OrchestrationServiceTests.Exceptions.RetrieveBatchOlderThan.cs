// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchOlderThanIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = GetRandomNumber();

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        randomCancellationToken);

            EventArchiveV2OrchestrationDependencyValidationException
                actualEventArchiveV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyValidationException>(
                        retrieveBatchTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyValidationException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOlderThanIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = GetRandomNumber();

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        randomCancellationToken);

            EventArchiveV2OrchestrationDependencyException
                actualEventArchiveV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                        retrieveBatchTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOlderThanIfExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = GetRandomNumber();

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();

            var failedEventArchiveV2OrchestrationServiceException =
                new FailedEventArchiveV2OrchestrationServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV2OrchestrationServiceException =
                new EventArchiveV2OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2OrchestrationServiceException);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        randomCancellationToken);

            EventArchiveV2OrchestrationServiceException
                actualEventArchiveV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationServiceException>(
                        retrieveBatchTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationServiceException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationServiceException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOlderThanIfTimeoutOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = GetRandomNumber();

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
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException
                actualEventArchiveV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                        retrieveBatchTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveBatchOlderThanAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = GetRandomNumber();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveBatchTask.AsTask);

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
