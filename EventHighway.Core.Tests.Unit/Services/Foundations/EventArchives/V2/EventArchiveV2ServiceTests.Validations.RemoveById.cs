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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
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
            ValueTask<EventArchiveV2> removeEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RemoveEventArchiveV2ByIdAsync(
                    invalidEventArchiveV2Id,
                    randomCancellationToken);

            EventArchiveV2ValidationException actualEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                    removeEventArchiveV2ByIdTask.AsTask);

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

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventArchiveV2IsNotFoundAndLogItAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventArchiveV2Id = GetRandomId();
            EventArchiveV2 noEventArchiveV2 = null;

            var notFoundEventArchiveV2Exception =
                new NotFoundEventArchiveV2Exception(
                    message: $"Could not find event archive with id: {someEventArchiveV2Id}.");

            var expectedEventArchiveV2ValidationException =
                new EventArchiveV2ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id,
                    cancellationToken))
                        .ReturnsAsync(noEventArchiveV2);

            // when
            ValueTask<EventArchiveV2> removeEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RemoveEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id,
                    cancellationToken);

            EventArchiveV2ValidationException actualEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ValidationException>(
                    removeEventArchiveV2ByIdTask.AsTask);

            // then
            actualEventArchiveV2ValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id,
                    cancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
