// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
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
        public async Task ShouldRetrieveAllEventParticipantV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventParticipantV2> randomEventParticipantV2s =
                CreateRandomEventParticipantV2s();

            IQueryable<EventParticipantV2> retrievedEventParticipantV2s =
                randomEventParticipantV2s;

            IQueryable<EventParticipantV2> expectedEventParticipantV2s =
                randomEventParticipantV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventParticipantV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventParticipantV2s);

            // when
            IQueryable<EventParticipantV2> actualEventParticipantV2s =
                await this.eventParticipantV2Service
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            // then
            actualEventParticipantV2s.Should().BeEquivalentTo(
                expectedEventParticipantV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
