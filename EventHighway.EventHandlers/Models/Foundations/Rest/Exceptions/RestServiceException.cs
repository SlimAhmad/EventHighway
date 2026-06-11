// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions
{
    internal class RestServiceException : Xeption
    {
        public RestServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
