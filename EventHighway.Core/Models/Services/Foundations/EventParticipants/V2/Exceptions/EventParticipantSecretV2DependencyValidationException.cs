// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    internal class EventParticipantSecretV2DependencyValidationException : Xeption
    {
        public EventParticipantSecretV2DependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
