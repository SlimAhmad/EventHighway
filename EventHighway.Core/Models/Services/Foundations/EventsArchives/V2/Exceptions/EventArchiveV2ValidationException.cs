// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions
{
    internal class EventArchiveV2ValidationException : Xeption
    {
        public EventArchiveV2ValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
