// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal partial class EventParticipantSecretV2Service : IEventParticipantSecretV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantSecretV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventParticipantSecretV2>> RetrieveAllEventParticipantSecretV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllEventParticipantSecretV2sAsync(
                cancellationToken);
        });

        public ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventParticipantSecretV2OnAddAsync(eventParticipantSecretV2);

            return await this.storageBroker.InsertEventParticipantSecretV2Async(
                eventParticipantSecretV2, cancellationToken);
        });
    }
}
