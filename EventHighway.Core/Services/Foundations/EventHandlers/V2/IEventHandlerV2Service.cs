// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal interface IEventHandlerV2Service
    {
        void RegisterEventHandlerV2(IEventHandler eventHandler);
        IEnumerable<IEventHandler> RetrieveAllEventHandlerV2s();
    }
}
