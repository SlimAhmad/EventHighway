// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task ShouldAddEventParticipantV2Async()
        {
            // given
            EventParticipantV2 randomEventParticipantV2 =
                CreateRandomEventParticipantV2();

            EventParticipantV2 inputEventParticipantV2 =
                randomEventParticipantV2;

            EventParticipantV2 expectedEventParticipantV2 =
                inputEventParticipantV2.DeepClone();

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.clientBroker
                    .AddEventParticipantV2Async(
                        inputEventParticipantV2);

            // then
            actualEventParticipantV2.Should()
                .BeEquivalentTo(expectedEventParticipantV2);

            await this.clientBroker
                .RemoveEventParticipantV2ByIdAsync(
                    inputEventParticipantV2.Id);
        }
    }
}
