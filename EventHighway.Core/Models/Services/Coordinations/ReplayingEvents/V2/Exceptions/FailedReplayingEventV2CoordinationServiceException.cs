// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions
{
    public class FailedReplayingEventV2CoordinationServiceException : Xeption
    {
        public FailedReplayingEventV2CoordinationServiceException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
