// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public async ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventParticipantV2Client
                .AddEventParticipantV2Async(eventParticipantV2, cancellationToken);

        public async ValueTask<IEnumerable<EventParticipantV2>>
            RetrieveAllEventParticipantV2sAsync(
                CancellationToken cancellationToken = default) =>
            await this.clientV2.EventParticipantV2Client
                .RetrieveAllEventParticipantV2sAsync(cancellationToken);

        public async ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventParticipantV2Client
                .RetrieveEventParticipantV2ByIdAsync(eventParticipantV2Id, cancellationToken);

        public async ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventParticipantV2Client
                .ModifyEventParticipantV2Async(eventParticipantV2, cancellationToken);
    }
}
