// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    public class InvalidEventParticipantV2Exception : Xeption
    {
        public InvalidEventParticipantV2Exception(string message)
            : base(message)
        { }
    }
}
