// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions
{
    public class TimeoutReplayingListenerEventV2OrchestrationException : Xeption
    {
        public TimeoutReplayingListenerEventV2OrchestrationException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
