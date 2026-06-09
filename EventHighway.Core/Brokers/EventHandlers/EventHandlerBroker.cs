// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.Core.Brokers.EventHandlers
{
    internal class EventHandlerBroker : IEventHandlerBroker
    {
        private readonly IEventHandler eventHandler;

        public EventHandlerBroker(IEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
        }

        public string Name => eventHandler.Name;
        public IEnumerable<string> RequiredParams => eventHandler.RequiredParams;

        public ValueTask<EventHandlerResult> HandleAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default) =>
            eventHandler.HandleAsync(content, handlerParams, cancellationToken);
    }
}
