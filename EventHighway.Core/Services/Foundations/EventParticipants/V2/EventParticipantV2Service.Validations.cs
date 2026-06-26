// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventParticipants.V2
{
    internal partial class EventParticipantV2Service
    {
        private async ValueTask ValidateEventParticipantV2OnAddAsync(EventParticipantV2 eventParticipantV2)
        {
            ValidateEventParticipantV2IsNotNull(eventParticipantV2);

            Validate(
                message: "Event participant is invalid, fix the errors and try again.",

                (Rule: IsNotDefault(eventParticipantV2.Id),
                Parameter: nameof(EventParticipantV2.Id)),

                (Rule: IsInvalid(eventParticipantV2.Name),
                Parameter: nameof(EventParticipantV2.Name)),

                (Rule: IsInvalid(eventParticipantV2.CreatedDate),
                Parameter: nameof(EventParticipantV2.CreatedDate)),

                (Rule: IsInvalid(eventParticipantV2.UpdatedDate),
                Parameter: nameof(EventParticipantV2.UpdatedDate)),

                (Rule: IsNotSameAs(
                    firstDate: eventParticipantV2.CreatedDate,
                    secondDate: eventParticipantV2.UpdatedDate,
                    secondDateName: nameof(EventParticipantV2.UpdatedDate)),

                Parameter: nameof(EventParticipantV2.CreatedDate)),

                (Rule: await IsNotRecentAsync(eventParticipantV2.CreatedDate),
                Parameter: nameof(EventParticipantV2.CreatedDate)),

                (Rule: IsInvalidIfSet(eventParticipantV2.ActiveFrom),
                Parameter: nameof(EventParticipantV2.ActiveFrom)),

                (Rule: IsInvalidIfSet(eventParticipantV2.ActiveTo),
                Parameter: nameof(EventParticipantV2.ActiveTo)),

                (Rule: IsRangeInvalid(eventParticipantV2.ActiveFrom, eventParticipantV2.ActiveTo),
                Parameter: nameof(EventParticipantV2.ActiveTo)));
        }

        private static void ValidateEventParticipantV2Id(Guid eventParticipantV2Id)
        {
            Validate(
                message: "Event participant is invalid, fix the errors and try again.",

                (Rule: IsInvalid(eventParticipantV2Id),
                Parameter: nameof(EventParticipantV2.Id)));
        }

        private static void ValidateEventParticipantV2Exists(EventParticipantV2 eventParticipantV2, Guid eventParticipantV2Id)
        {
            if (eventParticipantV2 is null)
            {
                throw new NotFoundEventParticipantV2Exception(
                    message: $"Could not find event participant with id: {eventParticipantV2Id}.");
            }
        }

        private static void ValidateEventParticipantV2IsNotNull(EventParticipantV2 eventParticipantV2)
        {
            if (eventParticipantV2 is null)
            {
                throw new NullEventParticipantV2Exception(
                    message: "Event participant is null.");
            }
        }

        private static dynamic IsNotDefault(Guid id) => new
        {
            Condition = id != Guid.Empty,
            Message = "Not required"
        };

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Required"
        };

        private static dynamic IsNotSameAs(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsInvalidIfSet(DateTimeOffset? date) => new
        {
            Condition = date.HasValue && date.Value == default,
            Message = "Required"
        };

        private static dynamic IsRangeInvalid(DateTimeOffset? activeFrom, DateTimeOffset? activeTo) => new
        {
            Condition = activeFrom.HasValue && activeTo.HasValue && activeTo.Value <= activeFrom.Value,
            Message = $"Date must be after {nameof(EventParticipantV2.ActiveFrom)}"
        };

        private async ValueTask<dynamic> IsNotRecentAsync(DateTimeOffset date) => new
        {
            Condition = await IsDateNotRecentAsync(date),
            Message = "Date is not recent"
        };

        private async ValueTask<bool> IsDateNotRecentAsync(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            TimeSpan timeDifference = currentDateTime.Subtract(value: date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private static void Validate(
            string message,
            params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventParticipantV2Exception = new InvalidEventParticipantV2Exception(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventParticipantV2Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventParticipantV2Exception.ThrowIfContainsErrors();
        }
    }
}
