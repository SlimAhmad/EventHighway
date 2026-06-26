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
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventParticipantSecretV2Id = Guid.Empty;

            var invalidEventParticipantSecretV2Exception =
                new InvalidEventParticipantSecretV2Exception(
                    message: "Event participant secret is invalid, fix the errors and try again.");

            invalidEventParticipantSecretV2Exception.AddData(
                key: nameof(EventParticipantSecretV2.Id),
                values: "Required");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantSecretV2Exception);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Service.RemoveEventParticipantSecretV2ByIdAsync(
                    invalidEventParticipantSecretV2Id, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfEventParticipantSecretV2IsNotFoundAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid nonExistingEventParticipantSecretV2Id = GetRandomId();
            EventParticipantSecretV2 nullEventParticipantSecretV2 = null;

            var notFoundEventParticipantSecretV2Exception =
                new NotFoundEventParticipantSecretV2Exception(
                    message: $"Could not find event participant secret with id: {nonExistingEventParticipantSecretV2Id}.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventParticipantSecretV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nullEventParticipantSecretV2);

            // when
            ValueTask<EventParticipantSecretV2> removeEventParticipantSecretV2ByIdTask =
                this.eventParticipantSecretV2Service.RemoveEventParticipantSecretV2ByIdAsync(
                    nonExistingEventParticipantSecretV2Id, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    removeEventParticipantSecretV2ByIdTask.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
