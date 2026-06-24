// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnReplayIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            BatchConfiguration batchConfiguration = CreateBatchConfiguration(GetRandomNumber());

            var expectedException =
                new ReplayingEventV2CoordinationDependencyValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, randomCancellationToken);

            ReplayingEventV2CoordinationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyValidationException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnReplayIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            BatchConfiguration batchConfiguration = CreateBatchConfiguration(GetRandomNumber());

            var expectedException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, randomCancellationToken);

            ReplayingEventV2CoordinationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnReplayIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            BatchConfiguration batchConfiguration = CreateBatchConfiguration(GetRandomNumber());
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutReplayingEventV2CoordinationException =
                new TimeoutReplayingEventV2CoordinationException(
                    message: "Failed replaying event coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: timeoutReplayingEventV2CoordinationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate,
                    TestContext.Current.CancellationToken);

            ReplayingEventV2CoordinationDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnReplayIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            BatchConfiguration batchConfiguration = CreateBatchConfiguration(GetRandomNumber());
            var serviceException = new Exception();

            var failedReplayingEventV2CoordinationServiceException =
                new FailedReplayingEventV2CoordinationServiceException(
                    message: "Failed replaying event coordination service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedException =
                new ReplayingEventV2CoordinationServiceException(
                    message: "Replaying event service error occurred, contact support.",
                    innerException: failedReplayingEventV2CoordinationServiceException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, randomCancellationToken);

            ReplayingEventV2CoordinationServiceException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationServiceException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    It.IsAny<Guid?>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(), It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
