// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions
{
    public class FailedEventListenerV2ProcessingServiceException : Xeption
    {
        public FailedEventListenerV2ProcessingServiceException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
