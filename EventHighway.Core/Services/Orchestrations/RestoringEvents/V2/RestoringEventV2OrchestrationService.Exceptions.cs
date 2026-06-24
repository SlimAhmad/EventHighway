// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.RestoringEvents.V2
{
    internal partial class RestoringEventV2OrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutRestoringEventV2OrchestrationException =
                    new TimeoutRestoringEventV2OrchestrationException(
                        message: "Failed restoring event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var restoringEventV2OrchestrationDependencyException =
                    new RestoringEventV2OrchestrationDependencyException(
                        message: "Restoring event dependency error occurred, contact support.",
                        innerException: timeoutRestoringEventV2OrchestrationException);

                await this.loggingBroker.LogErrorAsync(
                    restoringEventV2OrchestrationDependencyException);

                throw restoringEventV2OrchestrationDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullRestoringEventV2OrchestrationException nullRestoringEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullRestoringEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingValidationException listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
            catch (EventListenerV2ProcessingValidationException eventListenerV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingValidationException);
            }
            catch (EventListenerV2ProcessingDependencyValidationException eventListenerV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2ProcessingServiceException);
            }
            catch (ListenerEventV2ProcessingDependencyException listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    listenerEventV2ProcessingServiceException);
            }
            catch (EventListenerV2ProcessingDependencyException eventListenerV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingDependencyException);
            }
            catch (EventListenerV2ProcessingServiceException eventListenerV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedRestoringEventV2OrchestrationServiceException =
                    new FailedRestoringEventV2OrchestrationServiceException(
                        message: "Failed restoring event orchestration service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedRestoringEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<RestoringEventV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var restoringEventV2OrchestrationServiceException =
                new RestoringEventV2OrchestrationServiceException(
                    message: "Restoring event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                restoringEventV2OrchestrationServiceException);

            return restoringEventV2OrchestrationServiceException;
        }

        private async ValueTask<RestoringEventV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var restoringEventV2OrchestrationDependencyException =
                new RestoringEventV2OrchestrationDependencyException(
                    message: "Restoring event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                restoringEventV2OrchestrationDependencyException);

            return restoringEventV2OrchestrationDependencyException;
        }

        private async ValueTask<RestoringEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var restoringEventV2OrchestrationDependencyValidationException =
                new RestoringEventV2OrchestrationDependencyValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                restoringEventV2OrchestrationDependencyValidationException);

            return restoringEventV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<RestoringEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var restoringEventV2OrchestrationValidationException =
                new RestoringEventV2OrchestrationValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(restoringEventV2OrchestrationValidationException);

            return restoringEventV2OrchestrationValidationException;
        }
    }
}
