// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions
{
    internal class ReplayingListenerEventV2OrchestrationDependencyException : Xeption
    {
        public ReplayingListenerEventV2OrchestrationDependencyException(
            string message,
            Xeption innerException)
            : base(message, innerException)
        { }
    }
}
