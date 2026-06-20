// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventV2sIfTimeoutOccursAndLogItAsync()
        {
            // given
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();

            IEnumerable<Guid> someEventV2Ids =
                Enumerable.Range(0, GetRandomNumber()).Select(_ => Guid.NewGuid()).ToList();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutArchivingEventV2OrchestrationException =
                new TimeoutArchivingEventV2OrchestrationException(
                    message: "Failed archiving event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedArchivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutArchivingEventV2OrchestrationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        someEventV2Ids,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationDependencyException
                actualArchivingEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyException.Should().BeEquivalentTo(
                expectedArchivingEventV2OrchestrationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetrieveBatchOfListenerEventV2sIfValidationErrorOccursAndLogItAsync(
                Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();

            IEnumerable<Guid> someEventV2Ids =
                Enumerable.Range(0, GetRandomNumber()).Select(_ => Guid.NewGuid()).ToList();

            var expectedArchivingEventV2OrchestrationDependencyValidationException =
                new ArchivingEventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        someEventV2Ids,
                        randomCancellationToken);

            ArchivingEventV2OrchestrationDependencyValidationException
                actualArchivingEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyValidationException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationDependencyValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventV2sIfDependencyErrorOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();

            IEnumerable<Guid> someEventV2Ids =
                Enumerable.Range(0, GetRandomNumber()).Select(_ => Guid.NewGuid()).ToList();

            var expectedArchivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        someEventV2Ids,
                        randomCancellationToken);

            ArchivingEventV2OrchestrationDependencyException
                actualArchivingEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOfListenerEventV2sIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();

            IEnumerable<Guid> someEventV2Ids =
                Enumerable.Range(0, GetRandomNumber()).Select(_ => Guid.NewGuid()).ToList();

            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedArchivingEventV2OrchestrationServiceException =
                new FailedArchivingEventV2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedArchivingEventV2OrchestrationServiceException =
                new ArchivingEventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedArchivingEventV2OrchestrationServiceException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        someEventV2Ids,
                        randomCancellationToken);

            ArchivingEventV2OrchestrationServiceException
                actualArchivingEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationServiceException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationServiceException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationServiceException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
