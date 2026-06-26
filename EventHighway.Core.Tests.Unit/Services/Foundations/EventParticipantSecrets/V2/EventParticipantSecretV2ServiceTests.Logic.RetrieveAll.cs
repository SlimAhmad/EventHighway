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

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventParticipantSecretV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventParticipantSecretV2> randomEventParticipantSecretV2s =
                CreateRandomEventParticipantSecretV2s();

            IQueryable<EventParticipantSecretV2> retrievedEventParticipantSecretV2s =
                randomEventParticipantSecretV2s;

            IQueryable<EventParticipantSecretV2> expectedEventParticipantSecretV2s =
                randomEventParticipantSecretV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventParticipantSecretV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventParticipantSecretV2s);

            // when
            IQueryable<EventParticipantSecretV2> actualEventParticipantSecretV2s =
                await this.eventParticipantSecretV2Service
                    .RetrieveAllEventParticipantSecretV2sAsync(randomCancellationToken);

            // then
            actualEventParticipantSecretV2s.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventParticipantSecretV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
