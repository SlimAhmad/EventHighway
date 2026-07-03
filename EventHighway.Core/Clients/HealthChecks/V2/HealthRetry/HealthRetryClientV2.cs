// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetrySummaries.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.RetrySummaries.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health retry client implementation, handling retry-health summary
    /// retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthRetryClientV2 : IHealthRetryClientV2
    {
        private readonly IRetrySummaryV2OrchestrationService retrySummaryV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthRetryClientV2"/> class with the
        /// specified retry summary orchestration service.
        /// </summary>
        /// <param name="retrySummaryV2OrchestrationService">The orchestration service for
        /// retry-health summaries.</param>
        public HealthRetryClientV2(IRetrySummaryV2OrchestrationService retrySummaryV2OrchestrationService) =>
            this.retrySummaryV2OrchestrationService = retrySummaryV2OrchestrationService;

        /// <summary>
        /// Retrieves the current retry-health summary asynchronously by delegating to the
        /// coordination service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation.</param>
        /// <returns>A <see cref="ValueTask{RetryHealthSummaryV2}"/> representing the asynchronous
        /// operation that returns the retry-health summary.</returns>
        public async ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.retrySummaryV2OrchestrationService
                    .RetrieveRetryHealthV2Async(cancellationToken);
            }
            catch (RetrySummaryV2OrchestrationDependencyException
                retrySummaryV2OrchestrationDependencyException)
            {
                throw CreateHealthRetryClientV2DependencyException(
                    retrySummaryV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (RetrySummaryV2OrchestrationServiceException
                retrySummaryV2OrchestrationServiceException)
            {
                throw CreateHealthRetryClientV2DependencyException(
                    retrySummaryV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthRetryClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthRetryClientV2DependencyException
            CreateHealthRetryClientV2DependencyException(Xeption innerException)
        {
            return new HealthRetryClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthRetryClientV2ServiceException
            CreateHealthRetryClientV2ServiceException(Xeption innerException)
        {
            return new HealthRetryClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
