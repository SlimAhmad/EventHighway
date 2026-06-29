// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        // The deferred IQueryable is materialized inside the database gate (ToList) so its enumeration
        // never escapes the lock and hits the shared DbContext concurrently.
        public ValueTask<IQueryable<EventListenerV2>>
            RetrieveEventListenerV2sByEventAddressIdAsync(
                Guid eventAddressV2Id,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventListenerV2Client
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        eventAddressV2Id, cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);
    }
}
