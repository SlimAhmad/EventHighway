// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventParticipantSecretV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventParticipantSecretV2> randomEventParticipantSecretV2s =
                CreateRandomEventParticipantSecretV2s();

            IEnumerable<EventParticipantSecretV2> retrievedEventParticipantSecretV2s =
                randomEventParticipantSecretV2s;

            IEnumerable<EventParticipantSecretV2> expectedEventParticipantSecretV2s =
                retrievedEventParticipantSecretV2s;

            this.eventParticipantSecretV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventParticipantSecretV2s.AsQueryable());

            // when
            IEnumerable<EventParticipantSecretV2> actualEventParticipantSecretV2s =
                await this.eventParticipantSecretV2Client
                    .RetrieveAllEventParticipantSecretV2sAsync(
                        randomCancellationToken);

            // then
            actualEventParticipantSecretV2s.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2s);

            this.eventParticipantSecretV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantSecretV2sAsync(
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
