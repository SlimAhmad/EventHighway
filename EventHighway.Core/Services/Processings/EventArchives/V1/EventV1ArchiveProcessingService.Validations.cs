// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;

namespace EventHighway.Core.Services.Processings.EventArchives.V1
{
    internal partial class EventArchiveV1ProcessingService
    {
        private static void ValidateEventV1ArchiveIsNotNull(EventArchiveV1 eventV1Archive)
        {
            if (eventV1Archive is null)
            {
                throw new NullEventArchiveV1ProcessingException(
                    message: "Event archive is null.");
            }
        }
    }
}
