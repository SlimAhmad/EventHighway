// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions
{
    internal class ListenerEventArchiveV2ServiceException : Xeption
    {
        public ListenerEventArchiveV2ServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
