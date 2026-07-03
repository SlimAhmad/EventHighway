// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RetrySummaries.V2.Exceptions
{
    internal class RetrySummaryV2OrchestrationDependencyException : Xeption
    {
        public RetrySummaryV2OrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
