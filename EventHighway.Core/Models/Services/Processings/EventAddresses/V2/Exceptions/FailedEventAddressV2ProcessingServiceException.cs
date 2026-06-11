// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions
{
    public class FailedEventAddressV2ProcessingServiceException : Xeption
    {
        public FailedEventAddressV2ProcessingServiceException(
            string message,
            Exception innerException,
            IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
