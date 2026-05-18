// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V1
{
    internal partial class ListenerEventV1ArchiveService
    {
        private async ValueTask ValidateListenerEventV1ArchiveOnAddAsync(
            ListenerEventArchiveV1 listenerEventV1Archive)
        {
            ValidateListenerEventV1ArchiveIsNotNull(listenerEventV1Archive);

            Validate(
                (Rule: IsInvalid(listenerEventV1Archive.Id),
                Parameter: nameof(ListenerEventArchiveV1.Id)),

                (Rule: IsInvalid(listenerEventV1Archive.EventId),
                Parameter: nameof(ListenerEventArchiveV1.EventId)),

                (Rule: IsInvalid(listenerEventV1Archive.EventAddressId),
                Parameter: nameof(ListenerEventArchiveV1.EventAddressId)),

                (Rule: IsInvalid(listenerEventV1Archive.EventListenerId),
                Parameter: nameof(ListenerEventArchiveV1.EventListenerId)),

                (Rule: IsInvalid(listenerEventV1Archive.Status),
                Parameter: nameof(ListenerEventArchiveV1.Status)),

                (Rule: IsInvalid(listenerEventV1Archive.CreatedDate),
                Parameter: nameof(ListenerEventArchiveV1.CreatedDate)),

                (Rule: IsInvalid(listenerEventV1Archive.UpdatedDate),
                Parameter: nameof(ListenerEventArchiveV1.UpdatedDate)),

                (Rule: IsInvalid(listenerEventV1Archive.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV1.ArchivedDate)),

                (Rule: await IsNotRecentAsync(listenerEventV1Archive.ArchivedDate),
                Parameter: nameof(ListenerEventArchiveV1.ArchivedDate)));
        }

        private static void ValidateListenerEventV1ArchiveIsNotNull(
            ListenerEventArchiveV1 listenerEventV1Archive)
        {
            if (listenerEventV1Archive is null)
            {
                throw new NullListenerEventArchiveV1Exception(
                    message: "Listener event archive is null.");
            }
        }

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

        private static dynamic IsInvalid<T>(T value) => new
        {
            Condition = IsInvalidEnum(value) is true,
            Message = "Value is not recognized"
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

        private static bool IsInvalidEnum<T>(T enumValue)
        {
            bool isDefined = Enum.IsDefined(
                enumType: typeof(T),
                value: enumValue);

            return isDefined is false;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidListenerEventV1ArchiveException =
                new InvalidListenerEventArchiveV1Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidListenerEventV1ArchiveException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidListenerEventV1ArchiveException.ThrowIfContainsErrors();
        }
    }
}
