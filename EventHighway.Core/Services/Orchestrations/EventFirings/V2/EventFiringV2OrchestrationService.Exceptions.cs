// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventFirings.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.EventFirings.V2
{
    internal partial class EventFiringV2OrchestrationService
    {
        private delegate ValueTask<EventV2> ReturningEventV2Function();

        private async ValueTask<EventV2> TryCatch(
            ReturningEventV2Function returningEventV2Function)
        {
            try
            {
                return await returningEventV2Function();
            }
            catch (NullEventFiringV2OrchestrationException
                nullEventFiringV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullEventFiringV2OrchestrationException);
            }
            catch (EventListenerV2ProcessingValidationException
                eventListenerV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingValidationException);
            }
            catch (EventListenerV2ProcessingDependencyValidationException
                eventListenerV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingValidationException
                listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException
                listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
            catch (EventCallV2ProcessingValidationException
                eventCallV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingValidationException);
            }
            catch (EventCallV2ProcessingDependencyValidationException
                eventCallV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingDependencyValidationException);
            }
            catch (EventListenerV2ProcessingDependencyException
                eventListenerV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingDependencyException);
            }
            catch (EventListenerV2ProcessingServiceException
                eventListenerV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingServiceException);
            }
            catch (ListenerEventV2ProcessingDependencyException
                listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException
                listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingServiceException);
            }
            catch (EventCallV2ProcessingDependencyException
                eventCallV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV2ProcessingDependencyException);
            }
            catch (EventCallV2ProcessingServiceException
                eventCallV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventCallV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedEventFiringV2OrchestrationServiceException =
                    new FailedEventFiringV2OrchestrationServiceException(
                        message: "Failed event firing service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedEventFiringV2OrchestrationServiceException);
            }
        }

        private async ValueTask<EventFiringV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventFiringV2OrchestrationValidationException =
                new EventFiringV2OrchestrationValidationException(
                    message: "Event firing validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventFiringV2OrchestrationValidationException);

            return eventFiringV2OrchestrationValidationException;
        }

        private async ValueTask<EventFiringV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventFiringV2OrchestrationDependencyValidationException =
                new EventFiringV2OrchestrationDependencyValidationException(
                    message: "Event firing validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                eventFiringV2OrchestrationDependencyValidationException);

            return eventFiringV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<EventFiringV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var eventFiringV2OrchestrationDependencyException =
                new EventFiringV2OrchestrationDependencyException(
                    message: "Event firing dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(eventFiringV2OrchestrationDependencyException);

            return eventFiringV2OrchestrationDependencyException;
        }

        private async ValueTask<EventFiringV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var eventFiringV2OrchestrationServiceException =
                new EventFiringV2OrchestrationServiceException(
                    message: "Event firing service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventFiringV2OrchestrationServiceException);

            return eventFiringV2OrchestrationServiceException;
        }
    }
}
