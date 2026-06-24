// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions
{
    internal class RestoringEventV2OrchestrationDependencyValidationException : Xeption
    {
        public RestoringEventV2OrchestrationDependencyValidationException(
            string message,
            Xeption innerException)
            : base(message, innerException)
        { }
    }
}
