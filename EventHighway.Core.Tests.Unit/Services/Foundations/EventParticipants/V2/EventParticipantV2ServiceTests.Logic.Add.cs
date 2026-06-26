// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipants.V2
{
    public partial class EventParticipantV2ServiceTests
    {
        [Fact]
        public async Task ShouldAddEventParticipantV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2(
                    dates: randomDateTimeOffset);

            randomEventParticipantV2.Id = Guid.Empty;
            EventParticipantV2 inputEventParticipantV2 = randomEventParticipantV2;
            EventParticipantV2 insertedEventParticipantV2 = inputEventParticipantV2;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventParticipantV2Async(
                    inputEventParticipantV2, randomCancellationToken))
                        .ReturnsAsync(insertedEventParticipantV2);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2Service
                    .AddEventParticipantV2Async(
                        inputEventParticipantV2, randomCancellationToken);

            EventParticipantV2 expectedEventParticipantV2 =
                insertedEventParticipantV2.DeepClone();

            // then
            actualEventParticipantV2.Should().BeEquivalentTo(
                expectedEventParticipantV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantV2Async(
                    inputEventParticipantV2, randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
