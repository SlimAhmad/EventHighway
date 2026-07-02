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
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Processings.EventArchives.V2;
using EventHighway.Core.Services.Processings.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V2
{
    internal partial class EventArchiveV2OrchestrationService : IEventArchiveV2OrchestrationService
    {
        private readonly IListenerEventArchiveV2ProcessingService listenerEventArchiveV2ProcessingService;
        private readonly IEventArchiveV2ProcessingService eventArchiveV2ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV2OrchestrationService(
            IListenerEventArchiveV2ProcessingService listenerEventArchiveV2ProcessingService,
            IEventArchiveV2ProcessingService eventArchiveV2ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventArchiveV2ProcessingService = listenerEventArchiveV2ProcessingService;
            this.eventArchiveV2ProcessingService = eventArchiveV2ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<EventArchiveV2>> RetrieveBatchOfEventArchiveV2sOlderThanAsync(
            DateTimeOffset olderThan,
            int take,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventArchiveV2ProcessingService
                .RetrieveBatchOfEventArchiveV2sOlderThanAsync(olderThan, take, cancellationToken);
        });

        public ValueTask<IEnumerable<ListenerEventArchiveV2>> RetrieveBatchOfListenerEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            int skip,
            int take,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.listenerEventArchiveV2ProcessingService
                .RetrieveBatchOfListenerEventArchiveV2sAsync(
                    eventAddressId,
                    eventListenerIds,
                    startDate,
                    endDate,
                    skip,
                    take,
                    cancellationToken);
        });

        public ValueTask<IEnumerable<EventArchiveV2>> RetrieveEventArchiveV2sByIdsAsync(
            IEnumerable<Guid> eventArchiveIds,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveIdsIsNotNull(eventArchiveIds);

            IQueryable<EventArchiveV2> eventArchiveV2s =
                await this.eventArchiveV2ProcessingService
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken);

            return eventArchiveV2s
                .Where(eventArchiveV2 => eventArchiveIds.Contains(eventArchiveV2.Id))
                .ToList();
        });

        public ValueTask<IEnumerable<EventArchiveV2>> RetrieveBatchOfEventArchiveV2sMatchingAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            int skip,
            int take,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask BulkRemoveEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            await this.eventArchiveV2ProcessingService
                .BulkRemoveEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);
        });

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventArchiveV2ProcessingService
                .RetrieveAllEventArchiveV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.eventArchiveV2ProcessingService
                .RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.listenerEventArchiveV2ProcessingService
                .RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);
        });

        public ValueTask AddEventArchiveV2WithListenerEventArchiveV2sAsync(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveV2(eventArchiveV2);

            await this.eventArchiveV2ProcessingService.AddEventArchiveV2Async(eventArchiveV2, cancellationToken);

            foreach (ListenerEventArchiveV2 listenerEventArchiveV2 in eventArchiveV2.ListenerEventArchiveV2s)
            {
                await this.listenerEventArchiveV2ProcessingService
                    .AddListenerEventArchiveV2Async(listenerEventArchiveV2, cancellationToken);
            }
        });

        public ValueTask<IEnumerable<EventArchiveV2>> BulkAddEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            return await this.eventArchiveV2ProcessingService
                .BulkAddEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);
        });

        public ValueTask<IEnumerable<EventArchiveV2>> BulkAddEventArchiveV2sWithListenerEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);

            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s =
                eventArchiveV2s.SelectMany(eventArchiveV2 =>
                    eventArchiveV2.ListenerEventArchiveV2s).ToList();

            IEnumerable<ListenerEventArchiveV2> archivedListenerEventArchiveV2s =
                await this.listenerEventArchiveV2ProcessingService
                    .BulkAddListenerEventArchiveV2sAsync(listenerEventArchiveV2s, cancellationToken);

            var archivedListenerEventArchiveV2Ids =
                archivedListenerEventArchiveV2s.Select(listenerEventArchiveV2 =>
                    listenerEventArchiveV2.Id).ToHashSet();

            foreach (EventArchiveV2 eventArchiveV2 in eventArchiveV2s)
            {
                eventArchiveV2.ListenerEventArchiveV2s =
                    eventArchiveV2.ListenerEventArchiveV2s
                        .Where(listenerEventArchiveV2 =>
                            archivedListenerEventArchiveV2Ids.Contains(listenerEventArchiveV2.Id))
                        .ToList();
            }

            return eventArchiveV2s;
        });

        public ValueTask<IEnumerable<ListenerEventArchiveV2>> BulkAddListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventArchiveV2sIsNotNull(listenerEventArchiveV2s);

            return await this.listenerEventArchiveV2ProcessingService
                .BulkAddListenerEventArchiveV2sAsync(listenerEventArchiveV2s, cancellationToken);
        });
    }
}
