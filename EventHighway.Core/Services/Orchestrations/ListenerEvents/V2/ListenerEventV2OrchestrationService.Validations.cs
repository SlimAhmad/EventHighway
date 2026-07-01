// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.ListenerEvents.V2
{
    internal partial class ListenerEventV2OrchestrationService
    {
        private static void ValidateOnRetrieveBatchOfListenerEventV2sByEventIds(
            IEnumerable<Guid> eventV2Ids)
        {
            Validate(
                message: "Listener event is invalid, fix the errors and try again.",

                (Rule: IsNull(eventV2Ids),
                Parameter: nameof(eventV2Ids)));
        }

        private static dynamic IsNull(IEnumerable<Guid> value) => new
        {
            Condition = value is null,
            Message = "Value is required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventV2OrchestrationException =
                new InvalidListenerEventV2OrchestrationException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventV2OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventV2OrchestrationException.ThrowIfContainsErrors();
        }
    }
}
