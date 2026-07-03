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
using EventHighway.Core.Models.Services.Orchestrations.RagStatuses.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.RagStatuses.V2
{
    internal partial class RagStatusV2OrchestrationService
    {
        private delegate ValueTask<IEnumerable<HealthCheckItemV2>> ReturningHealthCheckItemsFunction();

        private async ValueTask<IEnumerable<HealthCheckItemV2>> TryCatch(
            ReturningHealthCheckItemsFunction returningHealthCheckItemsFunction)
        {
            try
            {
                return await returningHealthCheckItemsFunction();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.");

                var timeoutRagStatusV2OrchestrationException =
                    new TimeoutRagStatusV2OrchestrationException(
                        message: "Failed rag status orchestration timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: timeoutException.Data);

                var ragStatusV2OrchestrationDependencyException =
                    new RagStatusV2OrchestrationDependencyException(
                        message: "Rag status dependency error occurred, contact support.",
                        innerException: timeoutRagStatusV2OrchestrationException);

                await this.loggingBroker.LogErrorAsync(ragStatusV2OrchestrationDependencyException);
                throw ragStatusV2OrchestrationDependencyException;
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
                var failedRagStatusV2OrchestrationServiceException =
                    new FailedRagStatusV2OrchestrationServiceException(
                        message: "Failed rag status service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedRagStatusV2OrchestrationServiceException);
            }
        }

        private async ValueTask<RagStatusV2OrchestrationDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var ragStatusV2OrchestrationDependencyException =
                new RagStatusV2OrchestrationDependencyException(
                    message: "Rag status dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(ragStatusV2OrchestrationDependencyException);

            return ragStatusV2OrchestrationDependencyException;
        }

        private async ValueTask<RagStatusV2OrchestrationServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var ragStatusV2OrchestrationServiceException =
                new RagStatusV2OrchestrationServiceException(
                    message: "Rag status service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(ragStatusV2OrchestrationServiceException);

            return ragStatusV2OrchestrationServiceException;
        }
    }
}
