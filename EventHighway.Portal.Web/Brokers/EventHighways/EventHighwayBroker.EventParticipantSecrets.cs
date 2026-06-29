// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public async ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventParticipantSecretV2Client
                .AddEventParticipantSecretV2Async(eventParticipantSecretV2, cancellationToken);

        public async ValueTask<IEnumerable<EventParticipantSecretV2>>
            RetrieveAllEventParticipantSecretV2sAsync(
                CancellationToken cancellationToken = default) =>
            await this.clientV2.EventParticipantSecretV2Client
                .RetrieveAllEventParticipantSecretV2sAsync(cancellationToken);

        public async ValueTask<EventParticipantSecretV2> ModifyEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventParticipantSecretV2Client
                .ModifyEventParticipantSecretV2Async(eventParticipantSecretV2, cancellationToken);
    }
}
