// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
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
            catch (ArchivingEvent2OrchestrationValidationException
                archivingEvent2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    archivingEvent2OrchestrationValidationException);
            }
            catch (ArchivingEvent2OrchestrationDependencyValidationException
                archivingEvent2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    archivingEvent2OrchestrationDependencyValidationException);
            }
            catch (EventArchiveV1OrchestrationValidationException
                eventArchiveV1OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV1OrchestrationValidationException);
            }
            catch (EventArchiveV1OrchestrationDependencyValidationException
                eventArchiveV1OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV1OrchestrationDependencyValidationException);
            }
            catch (ArchivingEvent2OrchestrationDependencyException
                archivingEvent2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    archivingEvent2OrchestrationDependencyException);
            }
            catch (ArchivingEvent2OrchestrationServiceException
                archivingEvent2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    archivingEvent2OrchestrationServiceException);
            }
            catch (EventArchiveV1OrchestrationDependencyException
                eventArchiveV1OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV1OrchestrationDependencyException);
            }
            catch (EventArchiveV1OrchestrationServiceException
                eventArchiveV1OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV1OrchestrationServiceException);
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

        private async ValueTask<ArchivingEventV2CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var archivingEventV1CoordinationDependencyValidationException =
                new ArchivingEventV2CoordinationDependencyValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV1CoordinationDependencyValidationException);

            return archivingEventV1CoordinationDependencyValidationException;
        }

        private async ValueTask<ArchivingEventV2CoordinationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var archivingEventV1CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEventV1CoordinationDependencyException);

            return archivingEventV1CoordinationDependencyException;
        }

        private async ValueTask<ArchivingEventV2CoordinationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var archivingEventV1CoordinationServiceException =
                new ArchivingEventV2CoordinationServiceException(
                    message: "Archiving event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEventV1CoordinationServiceException);

            return archivingEventV1CoordinationServiceException;
        }
    }
}
