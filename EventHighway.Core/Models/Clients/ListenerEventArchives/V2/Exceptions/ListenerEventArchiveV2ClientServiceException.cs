// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Clients.ListenerEventArchives.V2.Exceptions
{
    public class ListenerEventArchiveV2ClientServiceException : Xeption
    {
        public ListenerEventArchiveV2ClientServiceException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
