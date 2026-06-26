// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal partial class EventParticipantSecretV2Service : IEventParticipantSecretV2Service
    {
        private readonly IStorageBroker storageBroker;

        public EventParticipantSecretV2Service(IStorageBroker storageBroker)
        {
            this.storageBroker = storageBroker;
        }

        public async ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            await this.storageBroker.InsertEventParticipantSecretV2Async(
                eventParticipantSecretV2, cancellationToken);
    }
}
