// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventListeners.V1.Exceptions
{
    public class EventListenerV1ProcessingValidationException : Xeption
    {
        public EventListenerV1ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
