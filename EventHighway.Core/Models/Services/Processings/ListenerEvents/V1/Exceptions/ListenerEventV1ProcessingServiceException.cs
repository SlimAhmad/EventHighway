// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions
{
    public class ListenerEventV1ProcessingServiceException : Xeption
    {
        public ListenerEventV1ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
