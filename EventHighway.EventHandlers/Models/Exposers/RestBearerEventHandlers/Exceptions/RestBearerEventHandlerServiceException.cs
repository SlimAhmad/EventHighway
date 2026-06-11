// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using EventHighway.Abstractions.EventHandlers.Exceptions;
using Xeptions;

namespace EventHighway.EventHandlers.Models.Exposers.RestBearerEventHandlers.Exceptions
{
    public class RestBearerEventHandlerServiceException : Xeption, IEventHandlerServiceException
    {
        public RestBearerEventHandlerServiceException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
