// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;

namespace EventHighway.Core.Services.Processings.EventArchives.V2
{
    internal partial class EventArchiveV2ProcessingService : IEventArchiveV2ProcessingService
    {
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV2ProcessingService(
            IEventArchiveV2Service eventArchiveV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.eventArchiveV2Service.RetrieveAllEventArchiveV2sAsync();
        });

        public ValueTask<IEnumerable<EventArchiveV2>> BulkAddEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            return await this.eventArchiveV2Service
                .BulkAddEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);
        });

        public ValueTask<IEnumerable<EventArchiveV2>> RetrieveBatchOfEventArchiveV2sOlderThanAsync(
            DateTimeOffset olderThan,
            int take) =>
        TryCatch(async () =>
        {
            ValidateOnRetrieveBatchOfEventArchiveV2sOlderThan(olderThan, take);

            IQueryable<EventArchiveV2> allEventArchiveV2s =
                await this.eventArchiveV2Service.RetrieveAllEventArchiveV2sAsync();

            IQueryable<EventArchiveV2> filteredEventArchiveV2s =
                allEventArchiveV2s.Where(archive => archive.ArchivedDate < olderThan);

            return take == 0
                ? filteredEventArchiveV2s.AsEnumerable()
                : filteredEventArchiveV2s.Take(take).AsEnumerable();
        });

        public ValueTask BulkRemoveEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            await this.eventArchiveV2Service
                .BulkRemoveEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);
        });

        public ValueTask<EventArchiveV2> AddEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2IsNotNull(eventArchiveV2);

            return await this.eventArchiveV2Service.AddEventArchiveV2Async(
                eventArchiveV2,
                cancellationToken);
        });
    }
}
