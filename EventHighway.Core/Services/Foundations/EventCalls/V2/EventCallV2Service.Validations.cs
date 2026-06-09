// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;

namespace EventHighway.Core.Services.Foundations.EventCalls.V2
{
    internal partial class EventCallV2Service
    {
        private void ValidateEventCallV2OnRun(EventCallV2 eventCallV2)
        {
            ValidateEventCallV2IsNotNull(eventCallV2);
            ValidateEventHandlerBrokersIsNotNull();

            Validate(
                message: "Event call is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventCallV2.HandlerName),
                Parameter: nameof(EventCallV2.HandlerName),
                Message: "Text required"),

                (Rule: IsInvalid(eventCallV2.HandlerConfigurations),
                Parameter: nameof(EventCallV2.HandlerConfigurations),
                Message: "Configuration required"),

                (Rule: IsInvalid(eventCallV2.Content),
                Parameter: nameof(EventCallV2.Content),
                Message: "Payload required"));

            ValidateHandlerCount(eventCallV2.HandlerName);
        }

        private static void ValidateEventCallV2IsNotNull(EventCallV2 eventCallV2)
        {
            if (eventCallV2 is null)
            {
                throw new NullEventCallV2Exception(
                    message: "Event call is null.");
            }
        }

        private void ValidateEventHandlerBrokersIsNotNull()
        {
            if (this.eventHandlerBrokers is null)
            {
                throw new HandlerNotFoundEventCallV2Exception(
                    message: "No event call handler was found, fix the errors and try again.");
            }
        }

        private void ValidateHandlerCount(string handlerName)
        {
            int count =
                this.eventHandlerBrokers.Count(
                    broker => broker.Name == handlerName);

            Validate(
                message: "EventHandlerBrokers on event call is invalid, fix the errors and try again.",

                (Rule: count == 0,
                Parameter: nameof(EventCallV2.HandlerName),
                Message: $"No handler found that matches '{handlerName}', " +
                    $"fix registrations and try again."),

                (Rule: count > 1,
                Parameter: nameof(EventCallV2.HandlerName),
                Message: $"Multiple providers found that matches '{handlerName}', " +
                    $"fix registrations and try again."));
        }

        private static bool IsInvalid(string text) =>
            string.IsNullOrWhiteSpace(text);

        private static bool IsInvalid(List<HandlerConfiguration> configurations) =>
            configurations is null;

        private static void Validate(
            string message,
            params (bool Rule, string Parameter, string Message)[] validations)
        {
            var invalidEventCallV2Exception =
                new InvalidEventCallV2Exception(message);

            foreach ((bool rule, string parameter, string errorMessage) in validations)
            {
                if (rule)
                {
                    invalidEventCallV2Exception.UpsertDataList(
                        key: parameter,
                        value: errorMessage);
                }
            }

            invalidEventCallV2Exception.ThrowIfContainsErrors();
        }
    }
}
