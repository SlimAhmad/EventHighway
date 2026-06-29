// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.EventHighways.V2;

namespace EventHighway.Portal.Web.Infrastructure
{
    // Builds the EventHighway V2 client once, lazily, and serialized behind a lock so the many
    // dashboard panels that first-touch the broker concurrently never construct it twice (which
    // would race on Database.Migrate() over the same Core database and fail every call).
    //
    // Unlike a cached Lazy<T>, a failed construction is NOT remembered: if the factory throws (e.g.
    // a LocalDB cold-start SqlException) the client stays null and the next request retries, so the
    // portal recovers once the database becomes reachable — no app restart required.
    public sealed class ClientV2Provider
    {
        private readonly Func<IClientV2> clientFactory;
        private readonly object gate = new();
        private IClientV2? client;

        public ClientV2Provider(Func<IClientV2> clientFactory) =>
            this.clientFactory = clientFactory;

        public IClientV2 GetClient()
        {
            IClientV2? current = this.client;

            if (current is not null)
            {
                return current;
            }

            lock (this.gate)
            {
                return this.client ??= this.clientFactory();
            }
        }
    }
}
