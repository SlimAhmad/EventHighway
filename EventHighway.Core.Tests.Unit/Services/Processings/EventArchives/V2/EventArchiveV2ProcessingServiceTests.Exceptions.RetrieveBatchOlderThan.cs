// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchOlderThanIfValidationErrorOccursAndLogItAsync(
            Xeption eventArchiveV2ValidationException)
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            var expectedEventArchiveV2ProcessingDependencyValidationException =
                new EventArchiveV2ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: eventArchiveV2ValidationException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync())
                    .ThrowsAsync(eventArchiveV2ValidationException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake);

            EventArchiveV2ProcessingDependencyValidationException
                actualEventArchiveV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyValidationException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOlderThanIfDependencyErrorOccursAndLogItAsync(
            Xeption eventArchiveV2DependencyException)
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            var expectedEventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: eventArchiveV2DependencyException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync())
                    .ThrowsAsync(eventArchiveV2DependencyException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake);

            EventArchiveV2ProcessingDependencyException
                actualEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOlderThanIfExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            var serviceException = new Exception();

            var failedEventArchiveV2ProcessingServiceException =
                new FailedEventArchiveV2ProcessingServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV2ProcessingServiceException =
                new EventArchiveV2ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2ProcessingServiceException);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake);

            EventArchiveV2ProcessingServiceException
                actualEventArchiveV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingServiceException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingServiceException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingServiceException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
