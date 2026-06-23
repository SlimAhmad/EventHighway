// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V2
{
    internal interface IEventArchiveV2OrchestrationService
    {
        ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask AddEventArchiveV2WithListenerEventArchiveV2sAsync(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<EventArchiveV2>> BulkAddEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<ListenerEventArchiveV2>> BulkAddListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveEventArchiveV2sAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<EventArchiveV2>> RetrieveBatchOfEventArchiveV2sOlderThanAsync(
            DateTimeOffset olderThan,
            int take,
            CancellationToken cancellationToken = default);
    }
}
