// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions
{
    public class TimeoutEventV2ProcessingException : Xeption
    {
        public TimeoutEventV2ProcessingException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
