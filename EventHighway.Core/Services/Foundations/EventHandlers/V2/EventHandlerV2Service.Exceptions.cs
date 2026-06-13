// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventHandlers.V2
{
    internal partial class EventHandlerV2Service
    {
        private delegate void ReturningVoidFunction();
        private delegate IEnumerable<IEventHandler> ReturningEventHandlersFunction();

        private IEnumerable<IEventHandler> TryCatch(ReturningEventHandlersFunction returningEventHandlersFunction)
        {
            try
            {
                return returningEventHandlersFunction();
            }
            catch (Exception serviceException)
            {
                var failedEventHandlerV2ServiceException =
                    new FailedEventHandlerV2ServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw CreateServiceException(failedEventHandlerV2ServiceException);
            }
        }

        private void TryCatch(ReturningVoidFunction returningVoidFunction)
        {
            try
            {
                returningVoidFunction();
            }
            catch (NullEventHandlerV2Exception nullEventHandlerV2Exception)
            {
                throw CreateValidationException(nullEventHandlerV2Exception);
            }
            catch (InvalidEventHandlerV2Exception invalidEventHandlerV2Exception)
            {
                throw CreateValidationException(invalidEventHandlerV2Exception);
            }
            catch (Exception serviceException)
            {
                var failedEventHandlerV2ServiceException =
                    new FailedEventHandlerV2ServiceException(
                        message: "Failed event handler service error occurred, contact support.",
                        innerException: serviceException,
                        data: serviceException.Data);

                throw CreateServiceException(failedEventHandlerV2ServiceException);
            }
        }

        private static EventHandlerV2ValidationException CreateValidationException(Xeption exception)
        {
            return new EventHandlerV2ValidationException(
                message: "Event handler validation error occurred, fix the errors and try again.",
                innerException: exception);
        }

        private static EventHandlerV2ServiceException CreateServiceException(Xeption exception)
        {
            return new EventHandlerV2ServiceException(
                message: "Event handler service error occurred, contact support.",
                innerException: exception);
        }
    }
}
