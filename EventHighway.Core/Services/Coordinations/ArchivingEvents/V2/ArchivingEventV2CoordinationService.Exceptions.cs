// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2CoordinationService
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
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutArchivingEventV2CoordinationException =
                    new TimeoutArchivingEventV2CoordinationException(
                        message: "Failed archiving event coordination timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutArchivingEventV2CoordinationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidArchivingEventV2CoordinationException
                invalidArchivingEventV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidArchivingEventV2CoordinationException);
            }
            catch (ArchivingEventV2OrchestrationValidationException
                archivingEventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    archivingEventV2OrchestrationValidationException);
            }
            catch (ArchivingEventV2OrchestrationDependencyValidationException
                archivingEventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    archivingEventV2OrchestrationDependencyValidationException);
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
            catch (ArchivingEventV2OrchestrationDependencyException
                archivingEventV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    archivingEventV2OrchestrationDependencyException);
            }
            catch (ArchivingEventV2OrchestrationServiceException
                archivingEventV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    archivingEventV2OrchestrationServiceException);
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
            catch (AggregateException aggregateException)
            {
                var failedArchivingEventV2CoordinationServiceException =
                    new FailedArchivingEventV2CoordinationServiceException(
                        message: "Failed archiving event service error occurred, contact support.",
                        innerException: aggregateException,
                        data: aggregateException.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedArchivingEventV2CoordinationServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEventV2CoordinationServiceException =
                    new FailedArchivingEventV2CoordinationServiceException(
                        message: "Failed archiving event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedArchivingEventV2CoordinationServiceException);
            }
        }

        private async ValueTask<ArchivingEventV2CoordinationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var archivingEventV2CoordinationValidationException =
                new ArchivingEventV2CoordinationValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2CoordinationValidationException);

            return archivingEventV2CoordinationValidationException;
        }

        private async ValueTask<ArchivingEventV2CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var archivingEventV2CoordinationDependencyValidationException =
                new ArchivingEventV2CoordinationDependencyValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV2CoordinationDependencyValidationException);

            return archivingEventV2CoordinationDependencyValidationException;
        }

        private async ValueTask<ArchivingEventV2CoordinationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var archivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2CoordinationDependencyException);

            return archivingEventV2CoordinationDependencyException;
        }

        private async ValueTask<ArchivingEventV2CoordinationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var archivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV2CoordinationDependencyException);

            return archivingEventV2CoordinationDependencyException;
        }

        private async ValueTask<ArchivingEventV2CoordinationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var archivingEventV2CoordinationServiceException =
                new ArchivingEventV2CoordinationServiceException(
                    message: "Archiving event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV2CoordinationServiceException);

            return archivingEventV2CoordinationServiceException;
        }
    }
}
