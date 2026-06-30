// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.ListenerEventArchives.Exceptions
{
    public class ListenerEventArchivesViewServiceException : Xeption
    {
        public ListenerEventArchivesViewServiceException(Xeption innerException)
            : base(
                message: "Listener event archives view service error occurred, contact support.",
                innerException)
        { }
    }
}
