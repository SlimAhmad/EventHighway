// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    internal class EventParticipantV2ValidationException : Xeption
    {
        public EventParticipantV2ValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
