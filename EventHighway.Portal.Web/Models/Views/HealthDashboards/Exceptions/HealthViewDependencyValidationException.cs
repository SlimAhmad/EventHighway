// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Portal.Web.Models.Views.HealthDashboards.Exceptions
{
    public class HealthViewDependencyValidationException : Xeption
    {
        public HealthViewDependencyValidationException(Xeption innerException)
            : base(
                message: "Health view dependency validation error occurred, " +
                    "fix the errors and try again.",
                innerException)
        { }
    }
}
