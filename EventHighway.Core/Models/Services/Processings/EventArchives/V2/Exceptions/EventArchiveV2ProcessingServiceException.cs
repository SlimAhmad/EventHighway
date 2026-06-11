// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions
{
    internal class EventArchiveV2ProcessingServiceException : Xeption
    {
        public EventArchiveV2ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
