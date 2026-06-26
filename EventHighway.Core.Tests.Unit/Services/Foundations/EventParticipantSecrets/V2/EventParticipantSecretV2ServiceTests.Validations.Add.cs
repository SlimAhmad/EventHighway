// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantSecretV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantSecretV2 nullEventParticipantSecretV2 = null;

            var nullEventParticipantSecretV2Exception =
                new NullEventParticipantSecretV2Exception(
                    message: "Event participant secret is null.");

            var expectedEventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantSecretV2Exception);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    nullEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ValidationException actualEventParticipantSecretV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ValidationException>(
                    addEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ValidationException.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
