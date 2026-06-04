// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Clients.Events.V1.Exceptions
{
    public class EventV1ClientDependencyValidationException : Xeption
    {
        public EventV1ClientDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
