// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Abstractions.Models.EventHandlers;
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
        TryCatch(async () =>
        {
            ValidateEventCallV2OnRun(eventCallV2);

            IEventHandlerBroker handler =
                this.eventHandlerBrokers.Single(
                    broker => broker.Name == eventCallV2.HandlerName);

            ValidateHandlerConfigurations(handler, eventCallV2.HandlerConfigurations);

            IReadOnlyDictionary<string, string> handlerParams =
                eventCallV2.HandlerConfigurations.ToDictionary(
                    handlerConfiguration => handlerConfiguration.Name,
                    handlerConfiguration => handlerConfiguration.Value);

            EventHandlerResult result =
                await handler.HandleAsync(
                    content: eventCallV2.Content,
                    handlerParams: handlerParams);

            eventCallV2.IsSuccess = result.Succeeded;
            eventCallV2.Response = result.Response;
            eventCallV2.ResponseCode = result.ErrorCode;
            eventCallV2.ResponseReasonPhrase = result.ErrorMessage;

            return eventCallV2;
        });
    }
}
