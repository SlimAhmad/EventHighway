// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpresso;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;

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

            if (HasMissingPromotedProperties(
                eventCallV2.RequiredPromotedProperties,
                eventCallV2.PromotedProperties))
            {
                eventCallV2.IsSuccess = false;
                eventCallV2.ResponseCode = "MissingRequiredMetadata";
                eventCallV2.ResponseMessage =
                    "One or more promoted properties could not be extracted from the event content. " +
                    "Check that the event listener is correctly configured. " +
                    "Promoted properties must match property names in the event content. (Case Sensitive)";

                return eventCallV2;
            }

            if (!string.IsNullOrEmpty(eventCallV2.FilterCriteria))
            {
                bool shouldHandle = EvaluateFilterCriteria(
                    eventCallV2.FilterCriteria,
                    eventCallV2.PromotedProperties,
                    out string filterError);

                if (filterError is not null)
                {
                    eventCallV2.IsSuccess = false;
                    eventCallV2.ResponseCode = "BadFilterCriteria";
                    eventCallV2.ResponseMessage =
                        "The filter criteria expression is invalid. " +
                        "Check that the expression is correctly formatted " +
                        "and uses a valid Dynamic Expresso expression syntax. " +
                        "See the Dynamic Expresso documentation for more details - " +
                        "https://github.com/dynamicexpresso/DynamicExpresso";

                    return eventCallV2;
                }

                if (!shouldHandle)
                {
                    eventCallV2.IsSuccess = true;
                    eventCallV2.ResponseCode = "SkippedNotMatchingFilter";
                    eventCallV2.ResponseMessage =
                        "Event was not handled because it did not match the listener's filter criteria.";

                    return eventCallV2;
                }
            }

            EventHandlerResult result =
                await handler.HandleAsync(
                    content: eventCallV2.Content,
                    cancellationToken: cancellationToken);

            eventCallV2.IsSuccess = result.IsSuccess;
            eventCallV2.Response = result.Response;
            eventCallV2.ResponseCode = result.ResponseCode;
            eventCallV2.ResponseMessage = result.ResponseMessage;

            return eventCallV2;
        });

        private static bool HasMissingPromotedProperties(
            IEnumerable<string> requiredPromotedProperties,
            List<PromotedProperty> promotedProperties)
        {
            if (requiredPromotedProperties is null)
                return false;

            return requiredPromotedProperties.Any(key =>
                !promotedProperties.Any(p =>
                    string.Equals(p.Name, key, StringComparison.OrdinalIgnoreCase)
                    && p.Value is not null));
        }

        private static bool EvaluateFilterCriteria(
            string filterCriteria,
            List<PromotedProperty> promotedProperties,
            out string error)
        {
            error = null;

            try
            {
                Dictionary<string, string> propertyDict =
                    (promotedProperties ?? new List<PromotedProperty>())
                        .ToDictionary(
                            p => p.Name,
                            p => p.Value,
                            StringComparer.OrdinalIgnoreCase);

                Func<string, string> meta = key =>
                    propertyDict.TryGetValue(key, out string val) ? val : null;

                var interpreter = new Interpreter();
                interpreter.SetFunction("meta", meta);

                return interpreter.Eval<bool>(filterCriteria);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
