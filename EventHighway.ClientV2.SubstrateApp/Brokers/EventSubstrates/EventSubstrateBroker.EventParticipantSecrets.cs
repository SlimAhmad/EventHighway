// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public ValueTask<EventParticipantSecretV2> AddParticipantSecretAsync(
            EventParticipantSecretV2 participantSecret,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventParticipantSecretV2Client
                .AddEventParticipantSecretV2Async(participantSecret, cancellationToken);
    }
}
