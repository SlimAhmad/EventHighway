// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions
{
    internal class ArchivingEventV2OrchestrationValidationException : Xeption
    {
        public ArchivingEventV2OrchestrationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
