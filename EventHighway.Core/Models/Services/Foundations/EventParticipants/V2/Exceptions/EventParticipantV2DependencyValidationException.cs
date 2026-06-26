// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    internal class EventParticipantV2DependencyValidationException : Xeption
    {
        public EventParticipantV2DependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
