// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        // Idempotent on the participant's (stable) Id: an existing row with the same Id is reused so
        // re-running the seed does not insert duplicate participants.
        public async ValueTask<EventParticipantV2> AddParticipantAsync(
            EventParticipantV2 participant,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<EventParticipantV2> existingParticipants =
                await this.eventHighwayClient.V2.EventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(cancellationToken);

            EventParticipantV2 maybeParticipant =
                existingParticipants.FirstOrDefault(
                    existing => existing.Id == participant.Id);

            return maybeParticipant
                ?? await this.eventHighwayClient.V2.EventParticipantV2Client
                    .AddEventParticipantV2Async(participant, cancellationToken);
        }
    }
}
