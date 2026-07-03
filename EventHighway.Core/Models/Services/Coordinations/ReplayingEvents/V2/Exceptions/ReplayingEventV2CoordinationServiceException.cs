// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions
{
    internal class ReplayingEventV2CoordinationServiceException : Xeption
    {
        public ReplayingEventV2CoordinationServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
