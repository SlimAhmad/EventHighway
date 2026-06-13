// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventArchives.V2
{
    internal partial class EventArchiveV2OrchestrationService
    {
        private static void ValidateEventArchiveV2(EventArchiveV2 eventArchiveV2)
        {
            ValidateEventArchiveV2IsNotNull(eventArchiveV2);
            ValidateListenerEventArchiveV2sAreNotNull(eventArchiveV2);
        }

        private static void ValidateEventArchiveV2IsNotNull(EventArchiveV2 eventArchiveV2)
        {
            if (eventArchiveV2 is null)
            {
                throw new NullEventArchiveV2OrchestrationException(
                    message: "Event archive is null.");
            }
        }

        private static void ValidateListenerEventArchiveV2sAreNotNull(EventArchiveV2 eventArchiveV2)
        {
            if (eventArchiveV2.ListenerEventArchiveV2s is null)
            {
                throw new NullListenerEventArchiveV2sOrchestrationException(
                    message: "Listener event archives are null.");
            }
        }
    }
}
