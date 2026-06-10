// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions
{
    internal class FailedRestServiceServiceException : Xeption
    {
        public FailedRestServiceServiceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
