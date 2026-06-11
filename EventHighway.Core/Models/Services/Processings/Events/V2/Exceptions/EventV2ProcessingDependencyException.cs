// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions
{
    internal class EventV2ProcessingDependencyException : Xeption
    {
        public EventV2ProcessingDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
