// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions
{
    public class NullEventArchiveV2sOrchestrationException : Xeption
    {
        public NullEventArchiveV2sOrchestrationException(string message)
            : base(message)
        { }
    }
}
