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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Core.Services.Foundations.EventArchives.V2
{
    internal partial class EventArchiveV2Service : IEventArchiveV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventArchiveV2> AddEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await ValidateEventArchiveV2OnAddAsync(eventArchiveV2);

            return await this.storageBroker.InsertEventArchiveV2Async(eventArchiveV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.storageBroker.SelectAllEventArchiveV2sAsync();
        });

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2Id(eventArchiveV2Id);

            return await this.storageBroker.SelectEventArchiveV2ByIdAsync(eventArchiveV2Id, cancellationToken);
        });

        public ValueTask<EventArchiveV2> RemoveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2Id(eventArchiveV2Id);

            EventArchiveV2 maybeEventArchiveV2 =
                await this.storageBroker.SelectEventArchiveV2ByIdAsync(eventArchiveV2Id, cancellationToken);

            ValidateEventArchiveV2Exists(maybeEventArchiveV2, eventArchiveV2Id);

            return await this.storageBroker.DeleteEventArchiveV2Async(maybeEventArchiveV2, cancellationToken);
        });
    }
}
