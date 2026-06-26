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

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventParticipantSecretV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventParticipantSecretV2Id = GetRandomId();
            Guid inputEventParticipantSecretV2Id = randomEventParticipantSecretV2Id;

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            EventParticipantSecretV2 retrievedEventParticipantSecretV2 =
                randomEventParticipantSecretV2;

            EventParticipantSecretV2 deletedEventParticipantSecretV2 =
                retrievedEventParticipantSecretV2;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                deletedEventParticipantSecretV2.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2Id, randomCancellationToken))
                        .ReturnsAsync(retrievedEventParticipantSecretV2);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventParticipantSecretV2Async(
                    retrievedEventParticipantSecretV2, randomCancellationToken))
                        .ReturnsAsync(deletedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Service
                    .RemoveEventParticipantSecretV2ByIdAsync(
                        inputEventParticipantSecretV2Id, randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    inputEventParticipantSecretV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventParticipantSecretV2Async(
                    retrievedEventParticipantSecretV2, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
