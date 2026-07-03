// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2OrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();
        private delegate ValueTask<IEnumerable<EventV2>> ReturningEventV2EnumerableFunction();

        private async ValueTask<IEnumerable<EventV2>> TryCatch(
            ReturningEventV2EnumerableFunction returningEventV2EnumerableFunction)
        {
            try
            {
                return await returningEventV2EnumerableFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutArchivingEventV2OrchestrationException =
                    new TimeoutArchivingEventV2OrchestrationException(
                        message: "Failed archiving event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutArchivingEventV2OrchestrationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidArchivingEventV2OrchestrationException invalidArchivingEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidArchivingEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEventV2OrchestrationServiceException =
                    new FailedArchivingEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedArchivingEventV2OrchestrationServiceException);
            }
        }

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

                var timeoutArchivingEventV2OrchestrationException =
                    new TimeoutArchivingEventV2OrchestrationException(
                        message: "Failed archiving event orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var archivingEventV2OrchestrationDependencyException =
                    new ArchivingEventV2OrchestrationDependencyException(
                        message: "Event dependency error occurred, contact support.",
                        innerException: timeoutArchivingEventV2OrchestrationException);

                await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationDependencyException);
                throw archivingEventV2OrchestrationDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (NullArchivingEventV2sOrchestrationException nullArchivingEventV2sOrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullArchivingEventV2sOrchestrationException);
            }
            catch (EventV2ProcessingValidationException eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (EventV2ProcessingDependencyException eventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingDependencyException);
            }
            catch (EventV2ProcessingServiceException eventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEventV2OrchestrationServiceException =
                    new FailedArchivingEventV2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedArchivingEventV2OrchestrationServiceException);
            }
        }

        private async ValueTask<ArchivingEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationValidationException =
                new ArchivingEventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationValidationException);

            return archivingEventV2OrchestrationValidationException;
        }

        private async ValueTask<ArchivingEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationDependencyValidationException =
                new ArchivingEventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationDependencyValidationException);

            return archivingEventV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<ArchivingEventV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationDependencyException);

            return archivingEventV2OrchestrationDependencyException;
        }

        private async ValueTask<ArchivingEventV2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationDependencyException);

            return archivingEventV2OrchestrationDependencyException;
        }

        private async ValueTask<ArchivingEventV2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var archivingEventV2OrchestrationServiceException =
                new ArchivingEventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2OrchestrationServiceException);

            return archivingEventV2OrchestrationServiceException;
        }
    }
}
