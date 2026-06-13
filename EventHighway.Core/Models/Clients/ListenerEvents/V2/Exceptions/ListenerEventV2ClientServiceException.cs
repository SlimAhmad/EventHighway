// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Clients.ListenerEvents.V2.Exceptions
{
    public class ListenerEventV2ClientServiceException : Xeption
    {
        public ListenerEventV2ClientServiceException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
