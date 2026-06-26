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
        public async Task ShouldRemoveEventParticipantV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventParticipantV2Id = GetRandomId();
            Guid inputEventParticipantV2Id = randomEventParticipantV2Id;

            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            EventParticipantV2 retrievedEventParticipantV2 =
                randomEventParticipantV2;

            EventParticipantV2 deletedEventParticipantV2 =
                retrievedEventParticipantV2;

            EventParticipantV2 expectedEventParticipantV2 =
                deletedEventParticipantV2.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    inputEventParticipantV2Id, randomCancellationToken))
                        .ReturnsAsync(retrievedEventParticipantV2);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventParticipantV2Async(
                    retrievedEventParticipantV2, randomCancellationToken))
                        .ReturnsAsync(deletedEventParticipantV2);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2Service
                    .RemoveEventParticipantV2ByIdAsync(
                        inputEventParticipantV2Id, randomCancellationToken);

            // then
            actualEventParticipantV2.Should().BeEquivalentTo(
                expectedEventParticipantV2);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    inputEventParticipantV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventParticipantV2Async(
                    retrievedEventParticipantV2, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
