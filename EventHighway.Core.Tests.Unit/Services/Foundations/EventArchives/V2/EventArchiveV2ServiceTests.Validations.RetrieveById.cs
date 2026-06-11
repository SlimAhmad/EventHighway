// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventArchiveV2Id = Guid.Empty;

            var invalidEventArchiveV2Exception =
                new InvalidEventArchiveV2Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV2Exception.AddData(
                key: nameof(EventArchiveV2.Id),
                values: "Required");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventArchiveV2Exception);

            // when
            ValueTask<EventArchiveV2> retrieveEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RetrieveEventArchiveV2ByIdAsync(
                    invalidEventArchiveV2Id,
                    cancellationToken);

            EventArchiveV2ValidationException actualEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                    retrieveEventArchiveV2ByIdTask.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
