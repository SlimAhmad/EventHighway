// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions
{
    public class NullArchivingListenerEventV2sOrchestrationException : Xeption
    {
        public NullArchivingListenerEventV2sOrchestrationException(string message)
            : base(message)
        { }
    }
}
