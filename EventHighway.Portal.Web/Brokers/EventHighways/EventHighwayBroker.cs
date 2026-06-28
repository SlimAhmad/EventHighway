// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventHighways.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker : IEventHighwayBroker
    {
        private readonly IClientV2 clientV2;

        public EventHighwayBroker(IClientV2 clientV2) =>
            this.clientV2 = clientV2;
    }
}
