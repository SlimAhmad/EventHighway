// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.DuplicateSummaries.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.DuplicateSummaries.V2
{
    internal partial class DuplicateSummaryV2OrchestrationService
    {
        private delegate ValueTask<DuplicateDetectionSummaryV2> ReturningDuplicateDetectionSummaryFunction();

        private async ValueTask<DuplicateDetectionSummaryV2> TryCatch(
            ReturningDuplicateDetectionSummaryFunction returningDuplicateDetectionSummaryFunction)
        {
            try
            {
                return await returningDuplicateDetectionSummaryFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutDuplicateSummaryV2OrchestrationException =
                    new TimeoutDuplicateSummaryV2OrchestrationException(
                        message: "Failed duplicate summary orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutDuplicateSummaryV2OrchestrationException);
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
                var failedDuplicateSummaryV2OrchestrationServiceException =
                    new FailedDuplicateSummaryV2OrchestrationServiceException(
                        message: "Failed duplicate summary service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedDuplicateSummaryV2OrchestrationServiceException);
            }
        }

        private async ValueTask<DuplicateSummaryV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var duplicateSummaryV2OrchestrationDependencyException =
                new DuplicateSummaryV2OrchestrationDependencyException(
                    message: "Duplicate summary dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                duplicateSummaryV2OrchestrationDependencyException);

            return duplicateSummaryV2OrchestrationDependencyException;
        }

        private async ValueTask<DuplicateSummaryV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var duplicateSummaryV2OrchestrationDependencyException =
                new DuplicateSummaryV2OrchestrationDependencyException(
                    message: "Duplicate summary dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(duplicateSummaryV2OrchestrationDependencyException);

            return duplicateSummaryV2OrchestrationDependencyException;
        }

        private async ValueTask<DuplicateSummaryV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var duplicateSummaryV2OrchestrationServiceException =
                new DuplicateSummaryV2OrchestrationServiceException(
                    message: "Duplicate summary service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(duplicateSummaryV2OrchestrationServiceException);

            return duplicateSummaryV2OrchestrationServiceException;
        }
    }
}
