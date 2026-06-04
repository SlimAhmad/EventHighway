// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public class NullListenerEventArchiveV1sOrchestrationException : Xeption
    {
        public NullListenerEventArchiveV1sOrchestrationException(string message)
            : base(message)
        { }
    }
}
