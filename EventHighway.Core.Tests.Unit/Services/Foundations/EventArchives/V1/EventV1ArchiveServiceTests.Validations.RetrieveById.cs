// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEventV1ArchiveId = Guid.Empty;

            var invalidEventV1ArchiveException =
                new InvalidEventArchiveV1Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.Id),
                values: "Required");

            var expectedEventV1ArchiveValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            // when
            ValueTask<EventArchiveV1> retrieveEventV1ArchiveByIdTask =
                this.eventV1ArchiveService.RetrieveEventArchiveV1ByIdAsync(
                    invalidEventV1ArchiveId);

            EventArchiveV1ValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    retrieveEventV1ArchiveByIdTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV1ByIdAsync(
                    It.IsAny<Guid>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
