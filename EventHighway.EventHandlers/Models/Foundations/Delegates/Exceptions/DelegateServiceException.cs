// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions
{
    internal class DelegateServiceException : Xeption
    {
        public DelegateServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
