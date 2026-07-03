// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RetrySummaries.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.RetrySummaries.V2
{
    internal partial class RetrySummaryV2OrchestrationService
    {
        private delegate ValueTask<RetryHealthSummaryV2> ReturningRetryHealthSummaryFunction();

        private async ValueTask<RetryHealthSummaryV2> TryCatch(
            ReturningRetryHealthSummaryFunction returningRetryHealthSummaryFunction)
        {
            try
            {
                return await returningRetryHealthSummaryFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutRetrySummaryV2OrchestrationException =
                    new TimeoutRetrySummaryV2OrchestrationException(
                        message: "Failed retry summary orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutRetrySummaryV2OrchestrationException);
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
                var failedRetrySummaryV2OrchestrationServiceException =
                    new FailedRetrySummaryV2OrchestrationServiceException(
                        message: "Failed retry summary service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedRetrySummaryV2OrchestrationServiceException);
            }
        }

        private async ValueTask<RetrySummaryV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var retrySummaryV2OrchestrationDependencyException =
                new RetrySummaryV2OrchestrationDependencyException(
                    message: "Retry summary dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                retrySummaryV2OrchestrationDependencyException);

            return retrySummaryV2OrchestrationDependencyException;
        }

        private async ValueTask<RetrySummaryV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var retrySummaryV2OrchestrationDependencyException =
                new RetrySummaryV2OrchestrationDependencyException(
                    message: "Retry summary dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(retrySummaryV2OrchestrationDependencyException);

            return retrySummaryV2OrchestrationDependencyException;
        }

        private async ValueTask<RetrySummaryV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var retrySummaryV2OrchestrationServiceException =
                new RetrySummaryV2OrchestrationServiceException(
                    message: "Retry summary service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(retrySummaryV2OrchestrationServiceException);

            return retrySummaryV2OrchestrationServiceException;
        }
    }
}
