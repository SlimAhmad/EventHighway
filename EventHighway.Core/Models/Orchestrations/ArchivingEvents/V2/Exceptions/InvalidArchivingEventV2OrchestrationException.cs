// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions
{
    public class InvalidArchivingEventV2OrchestrationException : Xeption
    {
        public InvalidArchivingEventV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
