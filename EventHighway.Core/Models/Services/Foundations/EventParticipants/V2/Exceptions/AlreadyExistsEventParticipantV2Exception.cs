// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions
{
    public class AlreadyExistsEventParticipantV2Exception : Xeption
    {
        public AlreadyExistsEventParticipantV2Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
