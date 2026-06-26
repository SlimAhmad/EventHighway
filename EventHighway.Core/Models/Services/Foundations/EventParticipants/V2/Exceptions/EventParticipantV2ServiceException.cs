// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    internal class EventParticipantV2ServiceException : Xeption
    {
        public EventParticipantV2ServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
