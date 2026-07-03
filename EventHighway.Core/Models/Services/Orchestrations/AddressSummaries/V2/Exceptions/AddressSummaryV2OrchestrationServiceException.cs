// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.AddressSummaries.V2.Exceptions
{
    internal class AddressSummaryV2OrchestrationServiceException : Xeption
    {
        public AddressSummaryV2OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
