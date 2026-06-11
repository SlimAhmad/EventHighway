// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions
{
    public class InvalidDelegateServiceException : Xeption
    {
        public InvalidDelegateServiceException(string message)
            : base(message)
        { }

        public InvalidDelegateServiceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
