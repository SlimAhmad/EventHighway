// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V2
{
    internal partial class EventArchiveV2ProcessingService
    {
        private static void ValidateEventArchiveV2IsNotNull(EventArchiveV2 eventArchiveV2)
        {
            if (eventArchiveV2 is null)
            {
                throw new NullEventArchiveV2ProcessingException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateEventArchiveV2sIsNotNull(IEnumerable<EventArchiveV2> eventArchiveV2s)
        {
            if (eventArchiveV2s is null)
            {
                throw new NullEventArchiveV2ProcessingException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateOnRetrieveBatchOfEventArchiveV2sOlderThan(
            DateTimeOffset olderThan,
            int take)
        {
            Validate(
                (Rule: IsInvalid(olderThan), Parameter: "OlderThan"),
                (Rule: IsInvalid(take), Parameter: "Take"));
        }

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

        private static dynamic IsInvalid(int take) => new
        {
            Condition = take < 0,
            Message = "Value must be greater than or equal to 0"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventArchiveV2ProcessingException =
                new InvalidEventArchiveV2ProcessingException(
                    message: "Event archive is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventArchiveV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventArchiveV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
