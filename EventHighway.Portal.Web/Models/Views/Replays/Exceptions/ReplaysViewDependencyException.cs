// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.Replays.Exceptions
{
    public class ReplaysViewDependencyException : Xeption
    {
        public ReplaysViewDependencyException(Xeption innerException)
            : base(
                message: "Replays view dependency error occurred, contact support.",
                innerException)
        { }
    }
}
