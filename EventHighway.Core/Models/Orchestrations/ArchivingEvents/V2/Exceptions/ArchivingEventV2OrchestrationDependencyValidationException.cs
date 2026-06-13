// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions
{
    internal class ArchivingEventV2OrchestrationDependencyValidationException : Xeption
    {
        public ArchivingEventV2OrchestrationDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
