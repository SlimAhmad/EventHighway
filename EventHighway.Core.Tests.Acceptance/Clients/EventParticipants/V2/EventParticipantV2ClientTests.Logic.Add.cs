// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using FluentAssertions;

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

            // when
            EventParticipantV2 actualEventParticipantV2 =
                await this.clientBroker
                    .AddEventParticipantV2Async(
                        inputEventParticipantV2);

            // then
            // service mutates inputEventParticipantV2.Id on Add; compare against it
            actualEventParticipantV2.Should()
                .BeEquivalentTo(inputEventParticipantV2);

            await this.clientBroker
                .RemoveEventParticipantV2ByIdAsync(
                    actualEventParticipantV2.Id);
        }
    }
}
