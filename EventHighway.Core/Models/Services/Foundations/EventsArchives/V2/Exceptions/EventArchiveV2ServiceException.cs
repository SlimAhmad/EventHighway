// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions
{
    internal class EventArchiveV2ServiceException : Xeption
    {
        public EventArchiveV2ServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
