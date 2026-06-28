// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Coordinations.ReplayingEvents.V2
{
    internal partial class ReplayingEventV2CoordinationService
    {
        private static void ValidateOnReplay(
            DateTimeOffset? startDate,
            DateTimeOffset? endDate)
        {
            Validate(
                (Rule: IsAfter(startDate, endDate), Parameter: "startDate"));
        }

        private static void ValidateOnTargetedReplay(
            Guid eventV2Id,
            IEnumerable<Guid> eventListenerIds)
        {
            Validate(
                (Rule: IsInvalid(eventV2Id), Parameter: "eventV2Id"),
                (Rule: IsInvalid(eventListenerIds), Parameter: "eventListenerIds"));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(IEnumerable<Guid> eventListenerIds) => new
        {
            Condition = eventListenerIds is null || eventListenerIds.Any() is false,
            Message = "Event listener ids are required"
        };

        private static dynamic IsAfter(DateTimeOffset? startDate, DateTimeOffset? endDate) => new
        {
            Condition = startDate.HasValue
                && endDate.HasValue
                && startDate.Value > endDate.Value,

            Message = "Date is later than endDate"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidReplayingEventV2CoordinationException =
                new InvalidReplayingEventV2CoordinationException(
                    message: "Replaying event is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidReplayingEventV2CoordinationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidReplayingEventV2CoordinationException.ThrowIfContainsErrors();
        }
    }
}
