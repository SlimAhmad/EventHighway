// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    public interface IMediaItemService
    {
        // A sample internal subscriber: this delegate event handler is wired to a listener on the
        // "NFlix-NewReleases" address, showing how an internal service consumes events off the
        // substrate. It receives only the event content — never the publisher's credentials.
        IEventHandler MediaItemReceivedEventHandler { get; }
    }
}
