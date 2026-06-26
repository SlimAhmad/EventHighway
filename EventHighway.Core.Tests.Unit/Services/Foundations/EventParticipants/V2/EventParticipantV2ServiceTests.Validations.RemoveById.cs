// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipants.V2
{
    public partial class EventParticipantV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventParticipantV2Id = Guid.Empty;

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.Id),
                values: "Required");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            // when
            ValueTask<EventParticipantV2> removeEventParticipantV2ByIdTask =
                this.eventParticipantV2Service.RemoveEventParticipantV2ByIdAsync(
                    invalidEventParticipantV2Id, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    removeEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventParticipantV2IsNotFoundAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid nonExistingEventParticipantV2Id = GetRandomId();
            EventParticipantV2 nullEventParticipantV2 = null;

            var notFoundEventParticipantV2Exception =
                new NotFoundEventParticipantV2Exception(
                    message: $"Could not find event participant with id: {nonExistingEventParticipantV2Id}.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventParticipantV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nullEventParticipantV2);

            // when
            ValueTask<EventParticipantV2> removeEventParticipantV2ByIdTask =
                this.eventParticipantV2Service.RemoveEventParticipantV2ByIdAsync(
                    nonExistingEventParticipantV2Id, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    removeEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
