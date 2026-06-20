// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService
    {
        private delegate ValueTask<IEnumerable<HealthCheckItemV2>> ReturningHealthCheckItemsFunction();

        private async ValueTask<IEnumerable<HealthCheckItemV2>> TryCatch(
            ReturningHealthCheckItemsFunction returningHealthCheckItemsFunction)
        {
            try
            {
                return await returningHealthCheckItemsFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutHealthV2CoordinationException =
                    new TimeoutHealthV2CoordinationException(
                        message: "Failed health coordination timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var healthV2CoordinationDependencyException =
                    new HealthV2CoordinationDependencyException(
                        message: "Health dependency error occurred, contact support.",
                        innerException: timeoutHealthV2CoordinationException);

                await this.loggingBroker.LogErrorAsync(healthV2CoordinationDependencyException);

                throw healthV2CoordinationDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventV2OrchestrationValidationException
                eventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationValidationException);
            }
            catch (EventV2OrchestrationDependencyValidationException
                eventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2OrchestrationDependencyValidationException);
            }
            catch (EventV2OrchestrationDependencyException
                eventV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationDependencyException);
            }
            catch (EventV2OrchestrationServiceException
                eventV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventV2OrchestrationServiceException);
            }
            catch (EventListenerV2OrchestrationValidationException
                eventListenerV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2OrchestrationValidationException);
            }
            catch (EventListenerV2OrchestrationDependencyValidationException
                eventListenerV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2OrchestrationDependencyValidationException);
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2OrchestrationDependencyException);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventListenerV2OrchestrationServiceException);
            }
            catch (EventArchiveV2OrchestrationValidationException
                eventArchiveV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2OrchestrationValidationException);
            }
            catch (EventArchiveV2OrchestrationDependencyValidationException
                eventArchiveV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2OrchestrationDependencyValidationException);
            }
            catch (EventArchiveV2OrchestrationDependencyException
                eventArchiveV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2OrchestrationDependencyException);
            }
            catch (EventArchiveV2OrchestrationServiceException
                eventArchiveV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2OrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedHealthV2CoordinationServiceException =
                    new FailedHealthV2CoordinationServiceException(
                        message: "Failed health service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedHealthV2CoordinationServiceException);
            }
        }

        private async ValueTask<HealthV2CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationDependencyValidationException =
                new HealthV2CoordinationDependencyValidationException(
                    message: "Health validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationDependencyValidationException);

            return healthV2CoordinationDependencyValidationException;
        }

        private async ValueTask<HealthV2CoordinationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    message: "Health dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationDependencyException);

            return healthV2CoordinationDependencyException;
        }

        private async ValueTask<HealthV2CoordinationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var healthV2CoordinationServiceException =
                new HealthV2CoordinationServiceException(
                    message: "Health service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(healthV2CoordinationServiceException);

            return healthV2CoordinationServiceException;
        }
    }
}

