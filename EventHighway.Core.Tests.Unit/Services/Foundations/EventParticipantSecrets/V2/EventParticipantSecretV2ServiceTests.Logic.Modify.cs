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
        public async Task ShouldModifyEventParticipantSecretV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            int randomDaysAgo = GetRandomNegativeNumber();

            EventParticipantSecretV2 randomEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2(randomDateTime);

            EventParticipantSecretV2 inputEventParticipantSecretV2 =
                randomEventParticipantSecretV2;

            inputEventParticipantSecretV2.CreatedDate =
                randomDateTime.AddDays(randomDaysAgo);

            EventParticipantSecretV2 storageEventParticipantSecretV2 =
                inputEventParticipantSecretV2.DeepClone();

            int randomSecondsAgo =
                GetRandomNegativeNumber();

            DateTimeOffset storageUpdatedDate =
                randomDateTime.AddSeconds(randomSecondsAgo);

            storageEventParticipantSecretV2.UpdatedDate =
                storageUpdatedDate;

            EventParticipantSecretV2 persistedEventParticipantSecretV2 =
                inputEventParticipantSecretV2;

            EventParticipantSecretV2 expectedEventParticipantSecretV2 =
                persistedEventParticipantSecretV2.DeepClone();

            Guid eventParticipantSecretV2Id = inputEventParticipantSecretV2.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    eventParticipantSecretV2Id, randomCancellationToken))
                        .ReturnsAsync(storageEventParticipantSecretV2);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2, randomCancellationToken))
                        .ReturnsAsync(persistedEventParticipantSecretV2);

            // when
            EventParticipantSecretV2 actualEventParticipantSecretV2 =
                await this.eventParticipantSecretV2Service
                    .ModifyEventParticipantSecretV2Async(
                        inputEventParticipantSecretV2, randomCancellationToken);

            // then
            actualEventParticipantSecretV2.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    eventParticipantSecretV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantSecretV2Async(
                    inputEventParticipantSecretV2, randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
