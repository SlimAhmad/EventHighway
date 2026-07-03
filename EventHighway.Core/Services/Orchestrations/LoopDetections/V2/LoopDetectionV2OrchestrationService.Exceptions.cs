// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.LoopDetections.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.LoopDetections.V2
{
    internal partial class LoopDetectionV2OrchestrationService
    {
        private delegate ValueTask<LoopDetectionSummaryV2> ReturningLoopDetectionSummaryFunction();

        private async ValueTask<LoopDetectionSummaryV2> TryCatch(
            ReturningLoopDetectionSummaryFunction returningLoopDetectionSummaryFunction)
        {
            try
            {
                return await returningLoopDetectionSummaryFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutLoopDetectionV2OrchestrationException =
                    new TimeoutLoopDetectionV2OrchestrationException(
                        message: "Failed loop detection orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var loopDetectionV2OrchestrationDependencyException =
                    new LoopDetectionV2OrchestrationDependencyException(
                        message: "Loop detection dependency error occurred, contact support.",
                        innerException: timeoutLoopDetectionV2OrchestrationException);

                await this.loggingBroker.LogErrorAsync(loopDetectionV2OrchestrationDependencyException);
                throw loopDetectionV2OrchestrationDependencyException;
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
            catch (EventArchiveV2DependencyException eventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2DependencyException);
            }
            catch (EventArchiveV2ServiceException eventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedLoopDetectionV2OrchestrationServiceException =
                    new FailedLoopDetectionV2OrchestrationServiceException(
                        message: "Failed loop detection service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedLoopDetectionV2OrchestrationServiceException);
            }
        }

        private async ValueTask<LoopDetectionV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var loopDetectionV2OrchestrationDependencyException =
                new LoopDetectionV2OrchestrationDependencyException(
                    message: "Loop detection dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(loopDetectionV2OrchestrationDependencyException);

            return loopDetectionV2OrchestrationDependencyException;
        }

        private async ValueTask<LoopDetectionV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var loopDetectionV2OrchestrationServiceException =
                new LoopDetectionV2OrchestrationServiceException(
                    message: "Loop detection service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(loopDetectionV2OrchestrationServiceException);

            return loopDetectionV2OrchestrationServiceException;
        }
    }
}
