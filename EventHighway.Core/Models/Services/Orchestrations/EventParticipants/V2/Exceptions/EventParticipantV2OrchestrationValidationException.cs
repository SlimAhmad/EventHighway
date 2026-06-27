// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions
{
    internal class EventParticipantV2OrchestrationValidationException : Xeption
    {
        public EventParticipantV2OrchestrationValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
