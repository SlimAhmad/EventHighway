// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions
{
    internal class EventHandlerV2ValidationException : Xeption
    {
        public EventHandlerV2ValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
