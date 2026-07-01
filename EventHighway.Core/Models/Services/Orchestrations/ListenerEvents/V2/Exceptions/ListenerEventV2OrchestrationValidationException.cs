// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ListenerEvents.V2.Exceptions
{
    internal class ListenerEventV2OrchestrationValidationException : Xeption
    {
        public ListenerEventV2OrchestrationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
