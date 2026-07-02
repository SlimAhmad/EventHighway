// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventFirings.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventFirings.V2
{
    internal partial class EventFiringV2OrchestrationService
    {
        private static void ValidateEventV2IsNotNull(EventV2 eventV2)
        {
            if (eventV2 is null)
            {
                throw new NullEventFiringV2OrchestrationException(
                    message: "Event is null.");
            }
        }
    }
}
