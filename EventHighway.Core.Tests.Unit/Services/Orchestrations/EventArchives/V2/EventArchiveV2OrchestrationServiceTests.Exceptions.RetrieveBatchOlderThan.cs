// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

            var expectedEventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake);

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
                    someTake),
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

            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    someOlderThan,
                    someTake))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake);

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
                    someTake),
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
                    someTake))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchTask =
                this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake);

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
                    someTake),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationServiceException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
