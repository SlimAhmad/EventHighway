// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class EventArchiveV1DependencyException : Xeption
    {
        public EventArchiveV1DependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
