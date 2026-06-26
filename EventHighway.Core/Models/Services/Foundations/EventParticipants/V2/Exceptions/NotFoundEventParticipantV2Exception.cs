// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    public class NotFoundEventParticipantV2Exception : Xeption
    {
        public NotFoundEventParticipantV2Exception(string message)
            : base(message)
        { }
    }
}
