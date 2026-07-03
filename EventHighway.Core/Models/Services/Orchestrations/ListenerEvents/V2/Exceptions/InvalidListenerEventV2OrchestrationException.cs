// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions
{
    public class InvalidListenerEventV2OrchestrationException : Xeption
    {
        public InvalidListenerEventV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
