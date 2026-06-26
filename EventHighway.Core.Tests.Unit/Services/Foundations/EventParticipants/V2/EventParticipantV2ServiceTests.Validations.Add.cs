// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 nullEventParticipantV2 = null;

            var nullEventParticipantV2Exception =
                new NullEventParticipantV2Exception(
                    message: "Event participant is null.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantV2Exception);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    nullEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should().BeEquivalentTo(
                expectedEventParticipantV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
