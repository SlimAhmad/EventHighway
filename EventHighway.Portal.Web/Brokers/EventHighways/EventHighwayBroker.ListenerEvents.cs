// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        // The deferred IQueryable is materialized inside the database gate (ToList) so its enumeration
        // never escapes the lock and hits the shared DbContext concurrently.
        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.ListenerEventV2Client
                    .RetrieveAllListenerEventV2sAsync(cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sWithEventListenerV2Async(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.ListenerEventV2Client
                    .RetrieveAllListenerEventV2sWithEventListenerV2Async(cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);

        public ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ListenerEventV2Client
                    .RemoveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken),
                cancellationToken);
    }
}
