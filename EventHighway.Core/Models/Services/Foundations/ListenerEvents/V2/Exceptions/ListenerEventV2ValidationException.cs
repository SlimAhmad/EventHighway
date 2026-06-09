// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions
{
    public class ListenerEventV2ValidationException : Xeption
    {
        public ListenerEventV2ValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
