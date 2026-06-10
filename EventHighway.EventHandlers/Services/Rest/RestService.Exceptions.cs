// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using RESTFulSense.Exceptions;
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
            catch (HttpResponseUrlNotFoundException httpResponseUrlNotFoundException)
            {
                throw CreateCriticalDependencyException(httpResponseUrlNotFoundException);
            }
            catch (HttpResponseUnauthorizedException httpResponseUnauthorizedException)
            {
                throw CreateCriticalDependencyException(httpResponseUnauthorizedException);
            }
            catch (HttpResponseForbiddenException httpResponseForbiddenException)
            {
                throw CreateCriticalDependencyException(httpResponseForbiddenException);
            }
            catch (HttpResponseMethodNotAllowedException httpResponseMethodNotAllowedException)
            {
                throw CreateCriticalDependencyException(httpResponseMethodNotAllowedException);
            }
        }

        private static RestServiceValidationException CreateValidationException(Xeption exception) =>
            new RestServiceValidationException(
                message: "Rest service validation error occurred, fix the errors and try again.",
                innerException: exception);

        private static RestServiceDependencyException CreateCriticalDependencyException(Xeption exception)
        {
            var failedRestServiceException =
                new FailedRestServiceException(
                    message: "Failed rest service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            return new RestServiceDependencyException(
                message: "Rest service dependency error occurred, contact support.",
                innerException: failedRestServiceException);
        }
    }
}
