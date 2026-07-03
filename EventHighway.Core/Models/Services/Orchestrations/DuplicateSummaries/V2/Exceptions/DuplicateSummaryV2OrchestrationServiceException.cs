// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.DuplicateSummaries.V2.Exceptions
{
    internal class DuplicateSummaryV2OrchestrationServiceException : Xeption
    {
        public DuplicateSummaryV2OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
