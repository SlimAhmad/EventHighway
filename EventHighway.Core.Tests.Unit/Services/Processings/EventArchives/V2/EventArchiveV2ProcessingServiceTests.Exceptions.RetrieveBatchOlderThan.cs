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
    }
}
