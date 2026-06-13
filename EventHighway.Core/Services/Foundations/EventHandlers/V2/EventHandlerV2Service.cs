// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal partial class EventHandlerV2Service : IEventHandlerV2Service
    {
        private readonly IEventHandlerBroker eventHandlerBroker;

        public EventHandlerV2Service(IEventHandlerBroker eventHandlerBroker)
        {
            this.eventHandlerBroker = eventHandlerBroker;
        }

        public void RegisterEventHandlerV2(IEventHandler eventHandler) =>
            TryCatch(() =>
            {
                ValidateEventHandlerV2OnRegister(eventHandler);
                this.eventHandlerBroker.Register(eventHandler);
            });

        public IEnumerable<IEventHandler> RetrieveAllEventHandlerV2s() =>
            TryCatch(() => this.eventHandlerBroker.GetAll());
    }
}
