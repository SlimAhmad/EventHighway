// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.ParticipantSummaries.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.ParticipantSummaries.V2
{
    internal partial class ParticipantSummaryV2OrchestrationService
    {
        private delegate ValueTask<IEnumerable<ParticipantSummaryV2>> ReturningParticipantSummariesFunction();

        private async ValueTask<IEnumerable<ParticipantSummaryV2>> TryCatch(
            ReturningParticipantSummariesFunction returningParticipantSummariesFunction)
        {
            try
            {
                return await returningParticipantSummariesFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutParticipantSummaryV2OrchestrationException =
                    new TimeoutParticipantSummaryV2OrchestrationException(
                        message: "Failed participant summary orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var participantSummaryV2OrchestrationDependencyException =
                    new ParticipantSummaryV2OrchestrationDependencyException(
                        message: "Participant summary dependency error occurred, contact support.",
                        innerException: timeoutParticipantSummaryV2OrchestrationException);

                await this.loggingBroker.LogErrorAsync(participantSummaryV2OrchestrationDependencyException);
                throw participantSummaryV2OrchestrationDependencyException;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventAddressV2DependencyException eventAddressV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventAddressV2DependencyException);
            }
            catch (EventAddressV2ServiceException eventAddressV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventAddressV2ServiceException);
            }
            catch (EventV2DependencyException eventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2DependencyException);
            }
            catch (EventV2ServiceException eventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedParticipantSummaryV2OrchestrationServiceException =
                    new FailedParticipantSummaryV2OrchestrationServiceException(
                        message: "Failed participant summary service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedParticipantSummaryV2OrchestrationServiceException);
            }
        }

        private async ValueTask<ParticipantSummaryV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var participantSummaryV2OrchestrationDependencyException =
                new ParticipantSummaryV2OrchestrationDependencyException(
                    message: "Participant summary dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(participantSummaryV2OrchestrationDependencyException);

            return participantSummaryV2OrchestrationDependencyException;
        }

        private async ValueTask<ParticipantSummaryV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var participantSummaryV2OrchestrationServiceException =
                new ParticipantSummaryV2OrchestrationServiceException(
                    message: "Participant summary service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(participantSummaryV2OrchestrationServiceException);

            return participantSummaryV2OrchestrationServiceException;
        }
    }
}
