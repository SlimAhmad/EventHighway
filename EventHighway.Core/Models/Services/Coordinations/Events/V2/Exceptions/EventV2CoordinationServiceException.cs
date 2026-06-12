// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions
{
    internal class EventV2CoordinationServiceException : Xeption
    {
        public EventV2CoordinationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
