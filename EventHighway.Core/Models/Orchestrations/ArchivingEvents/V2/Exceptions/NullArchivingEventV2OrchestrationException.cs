// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions
{
    public class NullArchivingEventV2OrchestrationException : Xeption
    {
        public NullArchivingEventV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
