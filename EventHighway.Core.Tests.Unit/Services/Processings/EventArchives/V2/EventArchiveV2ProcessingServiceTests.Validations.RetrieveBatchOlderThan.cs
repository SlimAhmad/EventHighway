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

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOlderThanIfParametersAreInvalidAndLogItAsync()
        {
            // given
            DateTimeOffset invalidOlderThan = default;
            int negativeTake = -1;

            var invalidEventArchiveV2ProcessingException =
                new InvalidEventArchiveV2ProcessingException(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV2ProcessingException.UpsertDataList(
                key: "OlderThan",
                value: "Required");

            invalidEventArchiveV2ProcessingException.UpsertDataList(
                key: "Take",
                value: "Value must be greater than or equal to 0");

            var expectedEventArchiveV2ProcessingValidationException =
                new EventArchiveV2ProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV2ProcessingException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        invalidOlderThan,
                        negativeTake);

            EventArchiveV2ProcessingValidationException
                actualEventArchiveV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingValidationException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
