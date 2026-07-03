// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RagStatuses.V2.Exceptions
{
    internal class RagStatusV2OrchestrationDependencyException : Xeption
    {
        public RagStatusV2OrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
