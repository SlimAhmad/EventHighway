// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions
{
    internal class EventListenerV2ProcessingValidationException : Xeption
    {
        public EventListenerV2ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
