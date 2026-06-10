// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using Xeptions;

namespace EventHighway.EventHandlers.Servies.Rest
{
    internal partial class RestService
    {
        private delegate ValueTask<EventHandlerResult> ReturningEventHandlerResultFunction();

        private async ValueTask<EventHandlerResult> TryCatch(
            ReturningEventHandlerResultFunction returningEventHandlerResultFunction)
        {
            try
            {
                return await returningEventHandlerResultFunction();
            }
            catch (InvalidRestServiceException invalidRestServiceException)
            {
                throw CreateValidationException(invalidRestServiceException);
            }
        }

        private static RestServiceValidationException CreateValidationException(Xeption exception) =>
            new RestServiceValidationException(
                message: "Rest service validation error occurred, fix the errors and try again.",
                innerException: exception);
    }
}
