// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions
{
    internal class EventParticipantV2OrchestrationServiceException : Xeption
    {
        public EventParticipantV2OrchestrationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
