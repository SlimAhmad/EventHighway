// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions
{
    public class NullReplayingListenerEventV2OrchestrationException : Xeption
    {
        public NullReplayingListenerEventV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
