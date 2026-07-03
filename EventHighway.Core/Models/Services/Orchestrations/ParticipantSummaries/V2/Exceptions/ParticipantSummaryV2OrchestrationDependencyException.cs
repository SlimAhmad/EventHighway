// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ParticipantSummaries.V2.Exceptions
{
    internal class ParticipantSummaryV2OrchestrationDependencyException : Xeption
    {
        public ParticipantSummaryV2OrchestrationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
