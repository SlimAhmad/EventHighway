// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions
{
    internal class ReplayingEventV2CoordinationDependencyException : Xeption
    {
        public ReplayingEventV2CoordinationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
