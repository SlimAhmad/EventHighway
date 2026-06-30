// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        // The deferred IQueryable is materialized inside the database gate (ToList) so its enumeration
        // never escapes the lock and hits the shared DbContext concurrently.
        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventV2Client
                    .RetrieveAllEventV2sAsync(cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sWithEventAddressV2Async(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(async client =>
                (await client.EventV2Client
                    .RetrieveAllEventV2sWithEventAddressV2Async(cancellationToken))
                    .ToList()
                    .AsQueryable(),
                cancellationToken);
    }
}
