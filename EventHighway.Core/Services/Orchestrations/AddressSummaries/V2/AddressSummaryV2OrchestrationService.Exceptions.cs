// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.AddressSummaries.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.AddressSummaries.V2
{
    internal partial class AddressSummaryV2OrchestrationService
    {
        private delegate ValueTask<IEnumerable<EventAddressSummaryV2>> ReturningEventAddressSummariesFunction();

        private async ValueTask<IEnumerable<EventAddressSummaryV2>> TryCatch(
            ReturningEventAddressSummariesFunction returningEventAddressSummariesFunction)
        {
            try
            {
                return await returningEventAddressSummariesFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutAddressSummaryV2OrchestrationException =
                    new TimeoutAddressSummaryV2OrchestrationException(
                        message: "Failed address summary orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutAddressSummaryV2OrchestrationException);
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
                var failedAddressSummaryV2OrchestrationServiceException =
                    new FailedAddressSummaryV2OrchestrationServiceException(
                        message: "Failed address summary service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedAddressSummaryV2OrchestrationServiceException);
            }
        }

        private async ValueTask<AddressSummaryV2OrchestrationDependencyException>
            CreateAndLogTimeoutDependencyExceptionAsync(Xeption exception)
        {
            var addressSummaryV2OrchestrationDependencyException =
                new AddressSummaryV2OrchestrationDependencyException(
                    message: "Address summary dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                addressSummaryV2OrchestrationDependencyException);

            return addressSummaryV2OrchestrationDependencyException;
        }

        private async ValueTask<AddressSummaryV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var addressSummaryV2OrchestrationDependencyException =
                new AddressSummaryV2OrchestrationDependencyException(
                    message: "Address summary dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(addressSummaryV2OrchestrationDependencyException);

            return addressSummaryV2OrchestrationDependencyException;
        }

        private async ValueTask<AddressSummaryV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var addressSummaryV2OrchestrationServiceException =
                new AddressSummaryV2OrchestrationServiceException(
                    message: "Address summary service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(addressSummaryV2OrchestrationServiceException);

            return addressSummaryV2OrchestrationServiceException;
        }
    }
}
