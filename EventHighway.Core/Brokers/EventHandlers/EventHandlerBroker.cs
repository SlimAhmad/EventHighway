// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.Core.Brokers.EventHandlers
{
    internal class EventHandlerBroker : IEventHandlerBroker
    {
        private readonly List<IEventHandler> eventHandlers = new List<IEventHandler>();

        public void Register(IEventHandler eventHandler) =>
            this.eventHandlers.Add(eventHandler);

        public IEnumerable<IEventHandler> GetAll() =>
            this.eventHandlers;
    }
}
