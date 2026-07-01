// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventV2sByEventIdsIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int inputTake = GetRandomNumber();
            IEnumerable<Guid> someEventV2Ids = CreateRandomEventV2Ids();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutListenerEventV2OrchestrationException =
                new TimeoutListenerEventV2OrchestrationException(
                    message: "Failed listener event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedListenerEventV2OrchestrationDependencyException =
                new ListenerEventV2OrchestrationDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: timeoutListenerEventV2OrchestrationException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sByEventIdsTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                        someEventV2Ids,
                        inputTake,
                        randomCancellationToken);

            ListenerEventV2OrchestrationDependencyException
                actualListenerEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationDependencyException>(
                        retrieveBatchOfListenerEventV2sByEventIdsTask.AsTask);

            // then
            actualListenerEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchOfListenerEventV2sByEventIdsIfValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int inputTake = GetRandomNumber();
            IEnumerable<Guid> someEventV2Ids = CreateRandomEventV2Ids();

            var expectedListenerEventV2OrchestrationDependencyValidationException =
                new ListenerEventV2OrchestrationDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sByEventIdsTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                        someEventV2Ids,
                        inputTake,
                        randomCancellationToken);

            ListenerEventV2OrchestrationDependencyValidationException
                actualListenerEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationDependencyValidationException>(
                        retrieveBatchOfListenerEventV2sByEventIdsTask.AsTask);

            // then
            actualListenerEventV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventV2sByEventIdsIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int inputTake = GetRandomNumber();
            IEnumerable<Guid> someEventV2Ids = CreateRandomEventV2Ids();

            var expectedListenerEventV2OrchestrationDependencyException =
                new ListenerEventV2OrchestrationDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sByEventIdsTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                        someEventV2Ids,
                        inputTake,
                        randomCancellationToken);

            ListenerEventV2OrchestrationDependencyException
                actualListenerEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationDependencyException>(
                        retrieveBatchOfListenerEventV2sByEventIdsTask.AsTask);

            // then
            actualListenerEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOfListenerEventV2sByEventIdsIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int inputTake = GetRandomNumber();
            IEnumerable<Guid> someEventV2Ids = CreateRandomEventV2Ids();
            var serviceException = new Exception();

            var failedListenerEventV2OrchestrationServiceException =
                new FailedListenerEventV2OrchestrationServiceException(
                    message: "Failed listener event orchestration service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV2OrchestrationServiceException =
                new ListenerEventV2OrchestrationServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sByEventIdsTask =
                this.listenerEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                        someEventV2Ids,
                        inputTake,
                        randomCancellationToken);

            ListenerEventV2OrchestrationServiceException
                actualListenerEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ListenerEventV2OrchestrationServiceException>(
                        retrieveBatchOfListenerEventV2sByEventIdsTask.AsTask);

            // then
            actualListenerEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2OrchestrationServiceException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
