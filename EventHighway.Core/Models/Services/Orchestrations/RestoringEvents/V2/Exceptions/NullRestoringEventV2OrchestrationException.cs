// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions
{
    public class NullRestoringEventV2OrchestrationException : Xeption
    {
        public NullRestoringEventV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
