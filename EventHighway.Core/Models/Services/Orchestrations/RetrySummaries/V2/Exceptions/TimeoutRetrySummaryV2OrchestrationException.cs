// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.RetrySummaries.V2.Exceptions
{
    public class TimeoutRetrySummaryV2OrchestrationException : Xeption
    {
        public TimeoutRetrySummaryV2OrchestrationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
