// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventListeners.V1.Exceptions
{
    public class EventListenerV1OrchestrationDependencyException : Xeption
    {
        public EventListenerV1OrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
