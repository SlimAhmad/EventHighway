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
        public async Task ShouldModifyEventParticipantV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            int randomDaysAgo = GetRandomNegativeNumber();

            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2(randomDateTime);

            EventParticipantV2 inputEventParticipantV2 =
                randomEventParticipantV2;

            inputEventParticipantV2.CreatedDate =
                randomDateTime.AddDays(randomDaysAgo);

            EventParticipantV2 storageEventParticipantV2 =
                inputEventParticipantV2.DeepClone();

            int randomSecondsAgo =
                GetRandomNegativeNumber();

            DateTimeOffset storageUpdatedDate =
                randomDateTime.AddSeconds(randomSecondsAgo);

            storageEventParticipantV2.UpdatedDate =
                storageUpdatedDate;

            EventParticipantV2 persistedEventParticipantV2 =
                inputEventParticipantV2;

            EventParticipantV2 expectedEventParticipantV2 =
                persistedEventParticipantV2.DeepClone();

            Guid eventParticipantV2Id = inputEventParticipantV2.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    eventParticipantV2Id, randomCancellationToken))
                        .ReturnsAsync(storageEventParticipantV2);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateEventParticipantV2Async(
                    inputEventParticipantV2, randomCancellationToken))
                        .ReturnsAsync(persistedEventParticipantV2);

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.eventParticipantV2Service
                    .ModifyEventParticipantV2Async(
                        inputEventParticipantV2, randomCancellationToken);

            // then
            actualEventParticipantV2.Should().BeEquivalentTo(
                expectedEventParticipantV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    eventParticipantV2Id, randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantV2Async(
                    inputEventParticipantV2, randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
