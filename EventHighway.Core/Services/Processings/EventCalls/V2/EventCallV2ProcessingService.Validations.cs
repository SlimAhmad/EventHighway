// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.EventCalls.V2
{
    internal partial class EventCallV2ProcessingService
    {
        private static void ValidateEventCallV2IsNotNull(EventCallV2 eventCallV2)
        {
            if (eventCallV2 is null)
            {
                throw new NullEventCallV2ProcessingException(
                    message: "Event call is null.");
            }
        }

        internal virtual void ValidatePromotedProperties(string promotedProperties) =>
            Validate(
                message: "Event call is invalid.",

                (Rule: IsInvalid(promotedProperties),
                Parameter: nameof(promotedProperties)));

        internal virtual void ValidatePromotePropertiesInputs(string content, string promotedProperties) =>
            Validate(
                message: "Event call is invalid.",

                (Rule: IsInvalid(content),
                Parameter: nameof(content)),

                (Rule: IsInvalid(promotedProperties),
                Parameter: nameof(promotedProperties)));

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static void Validate(string message, params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventCallV2ProcessingException =
                new InvalidEventCallV2ProcessingException(message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventCallV2ProcessingException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventCallV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
