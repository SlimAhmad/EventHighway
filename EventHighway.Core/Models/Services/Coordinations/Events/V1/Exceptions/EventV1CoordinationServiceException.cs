// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions
{
    public class EventV1CoordinationServiceException : Xeption
    {
        public EventV1CoordinationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
