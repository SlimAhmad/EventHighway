// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions
{
    public class EventParticipantV2ClientDependencyValidationException : Xeption
    {
        public EventParticipantV2ClientDependencyValidationException(
            string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
