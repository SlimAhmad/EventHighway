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
    /// <summary>
    /// Defines the contract for the EventHighway client, providing access to various event
    /// management operations including events, listeners, addresses, and their operations across
    /// different API versions.
    /// </summary>
    public interface IEventHighwayClient
    {
        /// <summary>
        /// Gets the client for managing event addresses.
        /// </summary>
        public IEventAddressesClient EventAddresses { get; }

        /// <summary>
        /// Gets the client for managing event listeners.
        /// </summary>
        public IEventListenersClient EventListeners { get; }

        /// <summary>
        /// Gets the client for managing events.
        /// </summary>
        public IEventsClient Events { get; }

        /// <summary>
        /// Gets the client for managing events in V1 API.
        /// </summary>
        public IEventV1sClient EventV1s { get; }

        /// <summary>
        /// Gets the client for managing events in V1 API with V1 operations.
        /// </summary>
        public IEventV1sClientV1 EventV1sV1 { get; }

        /// <summary>
        /// Gets the client for managing event addresses in V1 API.
        /// </summary>
        public IEventAddressesV1Client EventAddressV1s { get; }

        /// <summary>
        /// Gets the client for managing event listeners in V1 API.
        /// </summary>
        public IEventListenerV1sClient EventListenerV1s { get; }

        /// <summary>
        /// Gets the client for managing listener events in V1 API.
        /// </summary>
        public IListenerEventV1sClient ListenerEventV1s { get; }

        /// <summary>
        /// Gets the V2 API client for advanced event management and handler registration.
        /// </summary>
        public IClientV2 V2 { get; }
    }
}