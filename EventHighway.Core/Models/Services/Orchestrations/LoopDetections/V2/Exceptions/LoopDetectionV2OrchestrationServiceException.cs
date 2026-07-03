// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.LoopDetections.V2.Exceptions
{
    internal class LoopDetectionV2OrchestrationServiceException : Xeption
    {
        public LoopDetectionV2OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
