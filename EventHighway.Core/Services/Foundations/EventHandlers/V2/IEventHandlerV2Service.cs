// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal interface IEventHandlerV2Service
    {
        void RegisterEventHandlerV2(IEventHandler eventHandler);
    }
}
