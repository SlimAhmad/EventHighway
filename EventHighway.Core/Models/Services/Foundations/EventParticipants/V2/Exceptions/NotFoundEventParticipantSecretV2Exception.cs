// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    public class NotFoundEventParticipantSecretV2Exception : Xeption
    {
        public NotFoundEventParticipantSecretV2Exception(string message)
            : base(message)
        { }
    }
}
