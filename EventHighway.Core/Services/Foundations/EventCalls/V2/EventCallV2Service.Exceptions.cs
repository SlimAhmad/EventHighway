// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventCalls.V2
{
    internal partial class EventCallV2Service
    {
        private delegate ValueTask<EventCallV2> ReturningEventCallV2Function();

        private async ValueTask<EventCallV2> TryCatch(ReturningEventCallV2Function returningEventCallV2Function)
        {
            try
            {
                return await returningEventCallV2Function();
            }
            catch (NullEventCallV2Exception nullEventCallV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventCallV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventCallV2ServiceException =
                    new FailedEventCallV2ServiceException(
                        message: "Failed event call service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw await CreateAndLogServiceExceptionAsync(failedEventCallV2ServiceException);
            }
        }

        private async ValueTask<EventCallV2ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var eventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2ValidationException);

            return eventCallV2ValidationException;
        }

        private async ValueTask<EventCallV2ServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var eventCallV2ServiceException =
                new EventCallV2ServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventCallV2ServiceException);

            return eventCallV2ServiceException;
        }
    }
}
