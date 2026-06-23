// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2OrchestrationService
    {
        private static void ValidateOnRetrieveBatchOfListenerEventV2s(
            IEnumerable<Guid> eventV2Ids,
            BatchConfiguration batchConfiguration)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsNull(eventV2Ids),
                Parameter: nameof(eventV2Ids)),

                (Rule: IsInvalid(batchConfiguration.BatchSizeForBulkProcessing),
                Parameter: nameof(BatchConfiguration.BatchSizeForBulkProcessing)));
        }

        private static void ValidateOnRetrieveBatchOfQuarantined(
            BatchConfiguration batchConfiguration,
            LoopDetection loopDetection)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(batchConfiguration.BatchSizeForBulkProcessing),
                Parameter: nameof(BatchConfiguration.BatchSizeForBulkProcessing)),

                (Rule: IsNull(loopDetection),
                Parameter: nameof(LoopDetection)));
        }

        private static void ValidateOnRetrieveBatchOfDead(
            BatchConfiguration batchConfiguration)
        {
            Validate(
                message: "Event is invalid, fix the errors and try again.",

                (Rule: IsInvalid(batchConfiguration.BatchSizeForBulkProcessing),
                Parameter: nameof(BatchConfiguration.BatchSizeForBulkProcessing)));
        }

        private static dynamic IsNull(IEnumerable<Guid> value) => new
        {
            Condition = value is null,
            Message = "Value is required"
        };

        private static dynamic IsNull(LoopDetection value) => new
        {
            Condition = value is null,
            Message = "Value is required"
        };

        private static dynamic IsInvalid(int value) => new
        {
            Condition = value < 0,
            Message = "Value must be greater than or equal to 0"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidArchivingEventV2OrchestrationException =
                new InvalidArchivingEventV2OrchestrationException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidArchivingEventV2OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidArchivingEventV2OrchestrationException.ThrowIfContainsErrors();
        }

        private static void ValidateEventV2sIsNotNull(IEnumerable<EventV2> eventV2s)
        {
            if (eventV2s is null)
            {
                throw new NullArchivingEventV2sOrchestrationException(
                    message: "Events are null.");
            }
        }

        private static void ValidateEventV2IsNotNull(EventV2 eventV2)
        {
            if (eventV2 is null)
            {
                throw new NullArchivingEventV2OrchestrationException(
                    message: "Event is null.");
            }
        }

        private static void ValidateListenerEventV2sIsNotNull(IEnumerable<ListenerEventV2> listenerEventV2s)
        {
            if (listenerEventV2s is null)
            {
                throw new NullArchivingListenerEventV2sOrchestrationException(
                    message: "Listener events are null.");
            }
        }
    }
}
