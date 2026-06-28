// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Clients.EventHighways.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker : IEventHighwayBroker
    {
        private readonly Lazy<IClientV2> lazyClientV2;

        public EventHighwayBroker(Lazy<IClientV2> lazyClientV2) =>
            this.lazyClientV2 = lazyClientV2;

        // The underlying V2 client opens and migrates the Core database on construction.
        // Resolving it lazily — on first actual use inside a broker method — means a database
        // outage surfaces as an exception the calling view service can catch and report, rather
        // than crashing the whole app during dependency-injection resolution.
        private IClientV2 clientV2 => this.lazyClientV2.Value;
    }
}
