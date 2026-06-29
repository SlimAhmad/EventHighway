// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.EventListeners.Exceptions
{
    public class FailedEventListenersViewServiceException : Xeption
    {
        public FailedEventListenersViewServiceException(Exception innerException)
            : base(
                message: "Failed event listeners view service error occurred, contact support.",
                innerException)
        { }
    }
}
