// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Clients.EventParticipantSecrets.V2.Exceptions
{
    public class EventParticipantSecretV2ClientDependencyValidationException : Xeption
    {
        public EventParticipantSecretV2ClientDependencyValidationException(
            string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
