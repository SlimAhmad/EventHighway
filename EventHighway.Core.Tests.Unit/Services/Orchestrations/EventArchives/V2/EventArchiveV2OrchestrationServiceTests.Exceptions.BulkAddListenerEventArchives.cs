// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnBulkAddListenerEventArchiveV2sIfTimeoutOccursAndLogItAsync()
        {
            // given
            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

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
                    someListenerEventArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException
                actualEventArchiveV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnBulkAddListenerEventArchiveV2sIfValidationExceptionOccursAndLogItAsync(
                Xeption listenerEventArchiveV2ValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            var expectedEventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: listenerEventArchiveV2ValidationException.InnerException as Xeption);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(listenerEventArchiveV2ValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    randomCancellationToken);

            EventArchiveV2OrchestrationDependencyValidationException
                actualEventArchiveV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyValidationException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyValidationException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnBulkAddListenerEventArchiveV2sIfDependencyExceptionOccursAndLogItAsync(
                Xeption listenerEventArchiveV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: listenerEventArchiveV2DependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(listenerEventArchiveV2DependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    randomCancellationToken);

            EventArchiveV2OrchestrationDependencyException
                actualEventArchiveV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnBulkAddListenerEventArchiveV2sIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

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

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    randomCancellationToken);

            EventArchiveV2OrchestrationServiceException
                actualEventArchiveV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationServiceException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationServiceException);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationServiceException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
