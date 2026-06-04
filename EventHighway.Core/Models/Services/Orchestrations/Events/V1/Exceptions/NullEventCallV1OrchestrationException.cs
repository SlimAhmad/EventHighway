// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.Events.V1.Exceptions
{
    public class NullEventCallV1OrchestrationException : Xeption
    {
        public NullEventCallV1OrchestrationException(string message)
            : base(message)
        { }
    }
}
