// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask ArchiveEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ArchivingEventV2Client.ArchiveEventV2sAsync(cancellationToken),
                cancellationToken);

        public ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ArchivingEventV2Client.PurgeEventArchiveV2sAsync(
                    olderThan, cancellationToken),
                cancellationToken);

        // The deferred IQueryable is materialized inside the database gate (ToList) so its enumeration
        // never escapes the lock and hits the shared DbContext concurrently.
        public ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventArchiveV2Client
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);
    }
}
