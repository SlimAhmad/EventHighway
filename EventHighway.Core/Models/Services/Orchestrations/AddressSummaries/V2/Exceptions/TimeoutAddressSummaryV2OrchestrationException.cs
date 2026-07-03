// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.AddressSummaries.V2.Exceptions
{
    public class TimeoutAddressSummaryV2OrchestrationException : Xeption
    {
        public TimeoutAddressSummaryV2OrchestrationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
