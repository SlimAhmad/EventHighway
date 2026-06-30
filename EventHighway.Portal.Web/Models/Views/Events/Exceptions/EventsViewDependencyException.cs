// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.Events.Exceptions
{
    public class EventsViewDependencyException : Xeption
    {
        public EventsViewDependencyException(Xeption innerException)
            : base(
                message: "Events view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
