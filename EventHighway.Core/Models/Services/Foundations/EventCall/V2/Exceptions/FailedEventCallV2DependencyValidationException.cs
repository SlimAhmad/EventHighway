// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions
{
    public class FailedEventCallV2DependencyValidationException : Xeption
    {
        public FailedEventCallV2DependencyValidationException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
