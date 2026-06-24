// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
