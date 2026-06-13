// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions
{
    internal class ArchivingEvent2OrchestrationServiceException : Xeption
    {
        public ArchivingEvent2OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
