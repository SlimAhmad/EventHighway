// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions
{
    internal class EventCallV2ProcessingValidationException : Xeption
    {
        public EventCallV2ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
