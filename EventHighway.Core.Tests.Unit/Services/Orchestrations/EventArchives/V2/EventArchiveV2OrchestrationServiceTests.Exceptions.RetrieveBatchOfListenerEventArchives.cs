// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
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
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchOfListenerEventArchivesIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int someSkip = GetRandomNumber();
            int someTake = GetRandomNumber();

            var expectedException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: someSkip,
                        take: someTake,
                        cancellationToken: randomCancellationToken);

            EventArchiveV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyValidationException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventArchivesIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int someSkip = GetRandomNumber();
            int someTake = GetRandomNumber();

            var expectedException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: someSkip,
                        take: someTake,
                        cancellationToken: randomCancellationToken);

            EventArchiveV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventArchivesIfTimeoutOccursAndLogItAsync()
        {
            // given
            int someSkip = GetRandomNumber();
            int someTake = GetRandomNumber();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventArchiveV2OrchestrationException =
                new TimeoutEventArchiveV2OrchestrationException(
                    message: "Failed event archive orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: timeoutEventArchiveV2OrchestrationException);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: someSkip,
                        take: someTake,
                        cancellationToken: TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOfListenerEventArchivesIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int someSkip = GetRandomNumber();
            int someTake = GetRandomNumber();

            var serviceException = new Exception();

            var failedEventArchiveV2OrchestrationServiceException =
                new FailedEventArchiveV2OrchestrationServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedException =
                new EventArchiveV2OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2OrchestrationServiceException);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: someSkip,
                        take: someTake,
                        cancellationToken: randomCancellationToken);

            EventArchiveV2OrchestrationServiceException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationServiceException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveBatchOfListenerEventArchivesAsync()
        {
            // given
            int someSkip = GetRandomNumber();
            int someTake = GetRandomNumber();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: someSkip,
                        take: someTake,
                        cancellationToken: cancelledToken);

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

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
