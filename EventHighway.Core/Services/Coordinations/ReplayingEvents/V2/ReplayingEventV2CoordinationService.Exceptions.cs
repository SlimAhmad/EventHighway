// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.ReplayingEvents.V2
{
    internal partial class ReplayingEventV2CoordinationService
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

                var timeoutReplayingEventV2CoordinationException =
                    new TimeoutReplayingEventV2CoordinationException(
                        message: "Failed replaying event coordination timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutReplayingEventV2CoordinationException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidReplayingEventV2CoordinationException invalidReplayingEventV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidReplayingEventV2CoordinationException);
            }
            catch (EventArchiveV2OrchestrationValidationException eventArchiveV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2OrchestrationValidationException);
            }
            catch (EventArchiveV2OrchestrationDependencyValidationException eventArchiveV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2OrchestrationDependencyValidationException);
            }
            catch (RestoringEventV2OrchestrationValidationException restoringEventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    restoringEventV2OrchestrationValidationException);
            }
            catch (RestoringEventV2OrchestrationDependencyValidationException restoringEventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    restoringEventV2OrchestrationDependencyValidationException);
            }
            catch (ReplayingListenerEventV2OrchestrationValidationException replayingListenerEventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    replayingListenerEventV2OrchestrationValidationException);
            }
            catch (ReplayingListenerEventV2OrchestrationDependencyValidationException replayingListenerEventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    replayingListenerEventV2OrchestrationDependencyValidationException);
            }
            catch (EventArchiveV2OrchestrationDependencyException eventArchiveV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2OrchestrationDependencyException);
            }
            catch (EventArchiveV2OrchestrationServiceException eventArchiveV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    eventArchiveV2OrchestrationServiceException);
            }
            catch (RestoringEventV2OrchestrationDependencyException restoringEventV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    restoringEventV2OrchestrationDependencyException);
            }
            catch (RestoringEventV2OrchestrationServiceException restoringEventV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    restoringEventV2OrchestrationServiceException);
            }
            catch (ReplayingListenerEventV2OrchestrationDependencyException replayingListenerEventV2OrchestrationDependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    replayingListenerEventV2OrchestrationDependencyException);
            }
            catch (ReplayingListenerEventV2OrchestrationServiceException replayingListenerEventV2OrchestrationServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(
                    replayingListenerEventV2OrchestrationServiceException);
            }
            catch (Exception exception)
            {
                var failedReplayingEventV2CoordinationServiceException =
                    new FailedReplayingEventV2CoordinationServiceException(
                        message: "Failed replaying event coordination service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedReplayingEventV2CoordinationServiceException);
            }
        }

        private async ValueTask<ReplayingEventV2CoordinationServiceException>
            CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var replayingEventV2CoordinationServiceException =
                new ReplayingEventV2CoordinationServiceException(
                    message: "Replaying event service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                replayingEventV2CoordinationServiceException);

            return replayingEventV2CoordinationServiceException;
        }

        private async ValueTask<ReplayingEventV2CoordinationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var replayingEventV2CoordinationDependencyException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                replayingEventV2CoordinationDependencyException);

            return replayingEventV2CoordinationDependencyException;
        }

        private async ValueTask<ReplayingEventV2CoordinationDependencyException>
            CreateAndLogDependencyExceptionAsync(Xeption exception)
        {
            var replayingEventV2CoordinationDependencyException =
                new ReplayingEventV2CoordinationDependencyException(
                    message: "Replaying event dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                replayingEventV2CoordinationDependencyException);

            return replayingEventV2CoordinationDependencyException;
        }

        private async ValueTask<ReplayingEventV2CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var replayingEventV2CoordinationDependencyValidationException =
                new ReplayingEventV2CoordinationDependencyValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                replayingEventV2CoordinationDependencyValidationException);

            return replayingEventV2CoordinationDependencyValidationException;
        }

        private async ValueTask<ReplayingEventV2CoordinationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var replayingEventV2CoordinationValidationException =
                new ReplayingEventV2CoordinationValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                replayingEventV2CoordinationValidationException);

            return replayingEventV2CoordinationValidationException;
        }
    }
}
