// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions
{
    public class TimeoutReplayingEventV2CoordinationException : Xeption
    {
        public TimeoutReplayingEventV2CoordinationException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
