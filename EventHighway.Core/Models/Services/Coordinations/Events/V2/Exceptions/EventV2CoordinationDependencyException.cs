// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions
{
    internal class EventV2CoordinationDependencyException : Xeption
    {
        public EventV2CoordinationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
