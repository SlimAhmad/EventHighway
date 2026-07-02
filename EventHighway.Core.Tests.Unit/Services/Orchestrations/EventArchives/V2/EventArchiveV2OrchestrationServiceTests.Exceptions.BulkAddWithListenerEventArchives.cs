// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnBulkAddWithListenerEventArchivesIfTimeoutOccursAndLogItAsync()
        {
            // given
            IQueryable<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = someEventArchiveV2s;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventArchiveV2OrchestrationException =
                new TimeoutEventArchiveV2OrchestrationException(
                    message: "Failed event archive orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: timeoutEventArchiveV2OrchestrationException);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService
                    .BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException
                actualEventArchiveV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                        bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnBulkAddWithListenerEventArchivesIfValidationExceptionOccursAndLogItAsync(
                Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = someEventArchiveV2s;

            var expectedEventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService
                    .BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        randomCancellationToken);

            EventArchiveV2OrchestrationDependencyValidationException
                actualEventArchiveV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyValidationException>(
                        bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyValidationException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnBulkAddWithListenerEventArchivesIfDependencyExceptionOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = someEventArchiveV2s;

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService
                    .BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        randomCancellationToken);

            EventArchiveV2OrchestrationDependencyException
                actualEventArchiveV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                        bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnBulkAddWithListenerEventArchivesIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = someEventArchiveV2s;
            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventArchiveV2OrchestrationServiceException =
                new FailedEventArchiveV2OrchestrationServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedEventArchiveV2OrchestrationServiceException =
                new EventArchiveV2OrchestrationServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2OrchestrationServiceException);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService
                    .BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        randomCancellationToken);

            EventArchiveV2OrchestrationServiceException
                actualEventArchiveV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationServiceException>(
                        bulkAddEventArchiveV2sWithListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationServiceException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationServiceException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
