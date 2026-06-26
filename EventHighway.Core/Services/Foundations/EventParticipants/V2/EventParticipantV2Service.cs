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

namespace EventHighway.Core.Services.Foundations.EventParticipants.V2
{
    internal partial class EventParticipantV2Service : IEventParticipantV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllEventParticipantV2sAsync(
                cancellationToken);
        });

        public ValueTask<EventParticipantV2> RetrieveEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(new ReturningEventParticipantV2Function(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventParticipantV2Id(eventParticipantV2Id);

            EventParticipantV2 maybeEventParticipantV2 =
                await this.storageBroker.SelectEventParticipantV2ByIdAsync(
                    eventParticipantV2Id, cancellationToken);

            ValidateEventParticipantV2Exists(maybeEventParticipantV2, eventParticipantV2Id);

            return maybeEventParticipantV2;
        }));

        public async ValueTask<EventParticipantV2> ModifyEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventParticipantV2OnAddAsync(eventParticipantV2);
            eventParticipantV2.Id = Guid.NewGuid();

            return await this.storageBroker.InsertEventParticipantV2Async(
                eventParticipantV2, cancellationToken);
        });
    }
}
