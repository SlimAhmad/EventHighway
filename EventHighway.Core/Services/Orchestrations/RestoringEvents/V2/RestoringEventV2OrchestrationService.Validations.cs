// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.RestoringEvents.V2
{
    internal partial class RestoringEventV2OrchestrationService
    {
        private static void ValidateOnRestore(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s)
        {
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);
            ValidateListenerEventArchiveV2sIsNotNull(listenerEventArchiveV2s);
        }

        private static void ValidateOnGenerateReplay(
            IEnumerable<EventArchiveV2> eventArchiveV2s)
        {
            ValidateEventArchiveV2sIsNotNull(eventArchiveV2s);
        }

        private static void ValidateEventArchiveV2sIsNotNull(
            IEnumerable<EventArchiveV2> eventArchiveV2s)
        {
            if (eventArchiveV2s is null)
            {
                throw new NullRestoringEventV2OrchestrationException(
                    message: "Event archives are null.");
            }
        }

        private static void ValidateListenerEventArchiveV2sIsNotNull(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s)
        {
            if (listenerEventArchiveV2s is null)
            {
                throw new NullRestoringEventV2OrchestrationException(
                    message: "Listener event archives are null.");
            }
        }
    }
}
