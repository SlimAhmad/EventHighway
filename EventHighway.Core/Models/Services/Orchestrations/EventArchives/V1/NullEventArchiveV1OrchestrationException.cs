// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1
{
    public partial class NullEventArchiveV1OrchestrationException : Xeption
    {
        public NullEventArchiveV1OrchestrationException(string message)
            : base(message)
        { }
    }
}
