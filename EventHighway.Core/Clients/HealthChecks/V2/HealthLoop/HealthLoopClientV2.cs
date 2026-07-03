// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.LoopDetections.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.LoopDetections.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health loop client implementation, handling loop-detection summary
    /// retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthLoopClientV2 : IHealthLoopClientV2
    {
        private readonly ILoopDetectionV2OrchestrationService loopDetectionV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthLoopClientV2"/> class with the
        /// specified loop detection orchestration service.
        /// </summary>
        /// <param name="loopDetectionV2OrchestrationService">The orchestration service for
        /// loop-detection summaries.</param>
        public HealthLoopClientV2(ILoopDetectionV2OrchestrationService loopDetectionV2OrchestrationService) =>
            this.loopDetectionV2OrchestrationService = loopDetectionV2OrchestrationService;

        /// <summary>
        /// Retrieves the loop-detection summary asynchronously by delegating to the
        /// coordination service and handling any exceptions that occur.
        /// </summary>
        /// <param name="period">The period granularity to aggregate over.</param>
        /// <param name="windowStart">The inclusive UTC start of the window, or
        /// <see cref="DateTimeOffset.MinValue"/> for the current period.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation.</param>
        /// <returns>A <see cref="ValueTask{LoopDetectionSummaryV2}"/> representing the asynchronous
        /// operation that returns the loop-detection summary.</returns>
        public async ValueTask<LoopDetectionSummaryV2> RetrieveLoopDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(period, windowStart, cancellationToken);
            }
            catch (LoopDetectionV2OrchestrationDependencyException
                loopDetectionV2OrchestrationDependencyException)
            {
                throw CreateHealthLoopClientV2DependencyException(
                    loopDetectionV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (LoopDetectionV2OrchestrationServiceException
                loopDetectionV2OrchestrationServiceException)
            {
                throw CreateHealthLoopClientV2DependencyException(
                    loopDetectionV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthLoopClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthLoopClientV2DependencyException
            CreateHealthLoopClientV2DependencyException(Xeption innerException)
        {
            return new HealthLoopClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthLoopClientV2ServiceException
            CreateHealthLoopClientV2ServiceException(Xeption innerException)
        {
            return new HealthLoopClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
