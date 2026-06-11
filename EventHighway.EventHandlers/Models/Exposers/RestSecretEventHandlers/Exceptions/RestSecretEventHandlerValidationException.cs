// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using EventHighway.Abstractions.EventHandlers.Exceptions;
using Xeptions;

namespace EventHighway.EventHandlers.Models.Exposers.RestSecretEventHandlers.Exceptions
{
    public class RestSecretEventHandlerValidationException : Xeption, IEventHandlerValidationException
    {
        public RestSecretEventHandlerValidationException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
