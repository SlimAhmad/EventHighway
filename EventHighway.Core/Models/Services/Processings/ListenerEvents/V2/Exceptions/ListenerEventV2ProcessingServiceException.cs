// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions
{
    internal class ListenerEventV2ProcessingServiceException : Xeption
    {
        public ListenerEventV2ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
