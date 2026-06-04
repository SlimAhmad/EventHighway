// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Clients.EventAddresses.V1.Exceptions
{
    public class EventAddressV1ClientServiceException : Xeption
    {
        public EventAddressV1ClientServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
