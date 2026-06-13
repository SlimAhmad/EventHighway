// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions
{
    internal class EventHandlerV2ServiceException : Xeption
    {
        public EventHandlerV2ServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
