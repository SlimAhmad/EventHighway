// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2
{
    public class NullListenerEventArchiveV2sOrchestrationException : Xeption
    {
        public NullListenerEventArchiveV2sOrchestrationException(string message)
            : base(message)
        { }
    }
}
