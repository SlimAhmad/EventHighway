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

        public ValueTask BulkRemoveEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default) =>
            this.eventArchiveV2Service.BulkRemoveEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);

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
