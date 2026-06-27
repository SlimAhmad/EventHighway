// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions
{
    public class InvalidEventParticipantV2OrchestrationException : Xeption
    {
        public InvalidEventParticipantV2OrchestrationException(string message)
            : base(message)
        { }
    }
}
