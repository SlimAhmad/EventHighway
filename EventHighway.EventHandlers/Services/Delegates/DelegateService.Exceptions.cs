// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions;
using Xeptions;

namespace EventHighway.EventHandlers.Services.Delegates
{
    internal partial class DelegateService
    {
        private delegate ValueTask<EventHandlerResult> ReturningEventHandlerResultFunction();

        private async ValueTask<EventHandlerResult> TryCatch(
            ReturningEventHandlerResultFunction returningEventHandlerResultFunction)
        {
            try
            {
                return await returningEventHandlerResultFunction();
            }
            catch (InvalidDelegateServiceException invalidDelegateServiceException)
            {
                throw CreateValidationException(invalidDelegateServiceException);
            }
            catch (Exception exception)
            {
                throw CreateServiceException(exception);
            }
        }

        private static DelegateServiceValidationException CreateValidationException(
            Xeption innerException) =>
            new DelegateServiceValidationException(
                message: "Delegate service validation error occurred, fix the errors and try again.",
                innerException: innerException);

        private static DelegateServiceException CreateServiceException(Exception exception)
        {
            var failedDelegateServiceException =
                new FailedDelegateServiceException(
                    message: "Failed delegate service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            return new DelegateServiceException(
                message: "Delegate service error occurred, contact support.",
                innerException: failedDelegateServiceException);
        }
    }
}
