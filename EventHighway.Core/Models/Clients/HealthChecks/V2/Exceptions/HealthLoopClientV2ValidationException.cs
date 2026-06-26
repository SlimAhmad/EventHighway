// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions
{
    public class HealthLoopClientV2ValidationException : Xeption
    {
        public HealthLoopClientV2ValidationException(string message, Xeption innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
