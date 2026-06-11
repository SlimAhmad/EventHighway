// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2ProcessingService
    {
        private void ValidateListenerEventArchiveV2(ListenerEventArchiveV2 listenerEventArchiveV2)
        {
            if (listenerEventArchiveV2 is null)
            {
                throw new NullListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is null.");
            }
        }
    }
}
