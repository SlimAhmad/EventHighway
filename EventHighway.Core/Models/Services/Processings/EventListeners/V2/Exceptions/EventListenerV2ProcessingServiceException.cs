// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions
{
    internal class EventListenerV2ProcessingServiceException : Xeption
    {
        public EventListenerV2ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
