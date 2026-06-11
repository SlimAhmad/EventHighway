// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions
{
    internal class ListenerEventArchiveV2ProcessingServiceException : Xeption
    {
        public ListenerEventArchiveV2ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
