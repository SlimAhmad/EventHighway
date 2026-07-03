// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.DuplicateSummaries.V2.Exceptions
{
    public class FailedDuplicateSummaryV2OrchestrationServiceException : Xeption
    {
        public FailedDuplicateSummaryV2OrchestrationServiceException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
