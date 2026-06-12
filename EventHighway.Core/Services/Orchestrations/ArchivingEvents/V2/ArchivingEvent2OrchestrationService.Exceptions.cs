// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEvent2OrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullArchivingEvent2OrchestrationException nullArchivingEvent2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullArchivingEvent2OrchestrationException);
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
            catch (ListenerEventV2ProcessingValidationException listenerEventV2ProcessingValidationException)
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
            catch (ListenerEventV2ProcessingDependencyException listenerEventV2ProcessingDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingDependencyException);
            }
            catch (ListenerEventV2ProcessingServiceException listenerEventV2ProcessingServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventV2ProcessingServiceException);
            }
            catch (Exception exception)
            {
                var failedArchivingEvent2OrchestrationServiceException =
                    new FailedArchivingEvent2OrchestrationServiceException(
                        message: "Failed event service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedArchivingEvent2OrchestrationServiceException);
            }
        }

        private async ValueTask<ArchivingEvent2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var archivingEvent2OrchestrationValidationException =
                new ArchivingEvent2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEvent2OrchestrationValidationException);

            return archivingEvent2OrchestrationValidationException;
        }

        private async ValueTask<ArchivingEvent2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var archivingEvent2OrchestrationDependencyValidationException =
                new ArchivingEvent2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEvent2OrchestrationDependencyValidationException);

            return archivingEvent2OrchestrationDependencyValidationException;
        }

        private async ValueTask<ArchivingEvent2OrchestrationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var archivingEvent2OrchestrationDependencyException =
                new ArchivingEvent2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(archivingEvent2OrchestrationDependencyException);

            return archivingEvent2OrchestrationDependencyException;
        }

        private async ValueTask<ArchivingEvent2OrchestrationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var archivingEvent2OrchestrationServiceException =
                new ArchivingEvent2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(archivingEvent2OrchestrationServiceException);

            return archivingEvent2OrchestrationServiceException;
        }
    }
}
