// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions
{
    public class NotFoundEventListenerV1Exception : Xeption
    {
        public NotFoundEventListenerV1Exception(string message)
            : base(message)
        { }
    }
}
