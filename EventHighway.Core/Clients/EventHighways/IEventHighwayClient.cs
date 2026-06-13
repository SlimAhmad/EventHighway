// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Clients.EventAddresses;
using EventHighway.Core.Clients.EventAddresses.V1;
using EventHighway.Core.Clients.EventHighways.V2;
using EventHighway.Core.Clients.EventListeners;
using EventHighway.Core.Clients.EventListeners.V1;
using EventHighway.Core.Clients.Events;
using EventHighway.Core.Clients.Events.V1;
using EventHighway.Core.Clients.ListenerEvents.V1;

namespace EventHighway.Core.Clients.EventHighways
{
    public interface IEventHighwayClient
    {
        public IEventAddressesClient EventAddresses { get; }
        public IEventListenersClient EventListeners { get; }
        public IEventsClient Events { get; }
        public IEventV1sClient EventV1s { get; }
        public IEventV1sClientV1 EventV1sV1 { get; }
        public IEventAddressesV1Client EventAddressV1s { get; }
        public IEventListenerV1sClient EventListenerV1s { get; }
        public IListenerEventV1sClient ListenerEventV1s { get; }
        public IClientV2 V2 { get; }
    }
}