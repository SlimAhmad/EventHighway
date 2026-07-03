// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Orchestrations.LoopDetections.V2.Exceptions
{
    public class FailedLoopDetectionV2OrchestrationServiceException : Xeption
    {
        public FailedLoopDetectionV2OrchestrationServiceException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
