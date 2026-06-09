// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions
{
    public class EventListenerV2ServiceException : Xeption
    {
        public EventListenerV2ServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
