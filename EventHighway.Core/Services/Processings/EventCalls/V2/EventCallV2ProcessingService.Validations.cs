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

        internal virtual void ValidatePromotePropertiesInputs(string content, string promotedProperties)
        { }

        internal virtual void ValidatePromotedProperties(string promotedProperties)
        {
            var invalidEventCallV2ProcessingException =
                new InvalidEventCallV2ProcessingException(
                    message: "Event call is invalid.");

            if (string.IsNullOrWhiteSpace(promotedProperties))
            {
                invalidEventCallV2ProcessingException.AddData(
                    key: nameof(promotedProperties),
                    values: "Text is required");
            }

            invalidEventCallV2ProcessingException.ThrowIfContainsErrors();
        }
    }
}
