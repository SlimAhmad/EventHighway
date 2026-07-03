// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventFirings.V2.Exceptions
{
    public class NullEventFiringV2OrchestrationException : Xeption
    {
        public NullEventFiringV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
