// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions
{
    public class InvalidRestServiceException : Xeption
    {
        public InvalidRestServiceException(string message)
            : base(message)
        { }

        public InvalidRestServiceException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
