// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveEventParticipantV2ByIdAsync()
        {
            // given
            EventParticipantV2 randomEventParticipantV2 =
                await CreateRandomEventParticipantV2Async();

            EventParticipantV2 expectedEventParticipantV2 =
                randomEventParticipantV2.DeepClone();

            Guid inputEventParticipantV2Id =
                randomEventParticipantV2.Id;

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.clientBroker
                    .RetrieveEventParticipantV2ByIdAsync(
                        inputEventParticipantV2Id);

            // then
            actualEventParticipantV2.Should()
                .BeEquivalentTo(expectedEventParticipantV2);

            await this.clientBroker
                .RemoveEventParticipantV2ByIdAsync(
                    inputEventParticipantV2Id);
        }
    }
}
