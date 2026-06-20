// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions
{
    public class TimeoutEventV2CoordinationException : Xeption
    {
        public TimeoutEventV2CoordinationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
