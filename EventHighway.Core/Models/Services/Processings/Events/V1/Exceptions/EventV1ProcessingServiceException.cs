// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions
{
    public class EventV1ProcessingServiceException : Xeption
    {
        public EventV1ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
