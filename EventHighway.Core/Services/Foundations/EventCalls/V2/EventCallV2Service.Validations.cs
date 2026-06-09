// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventCalls.V2
{
    internal partial class EventCallV2Service
    {
        private static void ValidateEventCallV2OnRun(EventCallV2 eventCallV2)
        {
            ValidateEventCallV2IsNotNull(eventCallV2);
        }

        private static void ValidateEventCallV2IsNotNull(EventCallV2 eventCallV2)
        {
            if (eventCallV2 is null)
            {
                throw new NullEventCallV2Exception(
                    message: "Event call is null.");
            }
        }
    }
}
