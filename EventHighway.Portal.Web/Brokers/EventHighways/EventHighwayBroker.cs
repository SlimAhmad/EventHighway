// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventHighways.V2;
using EventHighway.Portal.Web.Infrastructure;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker : IEventHighwayBroker
    {
        private readonly ClientV2Provider clientV2Provider;

        public EventHighwayBroker(ClientV2Provider clientV2Provider) =>
            this.clientV2Provider = clientV2Provider;

        // The underlying V2 client opens and migrates the Core database on construction. Resolving
        // it on first actual use inside a broker method (via the provider, which builds it once and
        // retries after failure) means a database outage surfaces as an exception the calling view
        // service can catch and report, rather than crashing the app during DI resolution.
        private IClientV2 clientV2 => this.clientV2Provider.GetClient();
    }
}
