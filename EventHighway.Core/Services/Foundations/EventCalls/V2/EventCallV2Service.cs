// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;

namespace EventHighway.Core.Services.Foundations.EventCalls.V2
{
    internal partial class EventCallV2Service : IEventCallV2Service
    {
        private readonly IEventHandlerBroker eventHandlerBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventCallV2Service(
            IEventHandlerBroker eventHandlerBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHandlerBroker = eventHandlerBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventCallV2> RunEventCallV2Async(
            EventCallV2 eventCallV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventCallV2OnRun(eventCallV2);

            IEventHandler handler =
                this.eventHandlerBroker.GetAll()
                    .Single(h => h.Id == eventCallV2.HandlerId);

            ValidateHandlerConfigurations(handler, eventCallV2.HandlerConfigurations);

            IReadOnlyDictionary<string, string> handlerParams =
                eventCallV2.HandlerConfigurations.ToDictionary(
                    handlerConfiguration => handlerConfiguration.Name,
                    handlerConfiguration => handlerConfiguration.Value,
                    StringComparer.OrdinalIgnoreCase);

            EventHandlerResult result =
                await handler.HandleAsync(
                    content: eventCallV2.Content,
                    handlerParams: handlerParams,
                    cancellationToken: cancellationToken);

            eventCallV2.IsSuccess = result.IsSuccess;
            eventCallV2.Response = result.Response;
            eventCallV2.ResponseCode = result.ResponseCode;
            eventCallV2.ResponseMessage = result.ResponseMessage;

            return eventCallV2;
        });
    }
}
