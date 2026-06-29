// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.EventArchives.Exceptions
{
    public class FailedEventArchivesViewServiceException : Xeption
    {
        public FailedEventArchivesViewServiceException(Exception innerException)
            : base(
                message: "Failed event archives view service error occurred, contact support.",
                innerException)
        { }
    }
}
