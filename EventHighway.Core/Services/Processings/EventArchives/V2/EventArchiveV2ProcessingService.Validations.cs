// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
    }
}
