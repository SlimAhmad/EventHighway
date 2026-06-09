// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;

namespace EventHighway.Core.Services.Foundations.EventCalls.V2
{
    internal partial class EventCallV2Service : IEventCallV2Service
    {
        private readonly IEnumerable<IEventHandlerBroker> eventHandlerBrokers;
        private readonly ILoggingBroker loggingBroker;

        public EventCallV2Service(
            IEnumerable<IEventHandlerBroker> eventHandlerBrokers,
            ILoggingBroker loggingBroker)
        {
            this.eventHandlerBrokers = eventHandlerBrokers;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventCallV2> RunEventCallV2Async(EventCallV2 eventCallV2) =>
            throw new NotImplementedException();
    }
}
