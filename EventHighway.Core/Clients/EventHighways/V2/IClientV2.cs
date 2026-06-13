// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Clients.EventListeners.V2;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Clients.ListenerEvents.V2;

namespace EventHighway.Core.Clients.EventHighways.V2
{
    public interface IClientV2
    {
        IClientV2 RegisterEventHandler(IEventHandler eventHandler);
        IArchivingEventV2Client ArchivingEventV2Client { get; }
        IEventAddressV2Client EventAddressV2Client { get; }
        IEventListenerV2Client EventListenerV2Client { get; }
        IEventV2Client EventV2Client { get; }
        IListenerEventV2Client ListenerEventV2Client { get; }
    }
}
