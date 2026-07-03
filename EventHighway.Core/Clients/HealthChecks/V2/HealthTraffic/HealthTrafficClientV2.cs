// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Processings.Traffics.V2.Exceptions;
using EventHighway.Core.Services.Processings.Traffics.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health traffic client implementation, handling traffic snapshot
    /// retrieval while managing traffic processing service exceptions.
    /// </summary>
    internal class HealthTrafficClientV2 : IHealthTrafficClientV2
    {
        private readonly ITrafficV2ProcessingService trafficV2ProcessingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthTrafficClientV2"/> class with the
        /// specified traffic processing service.
        /// </summary>
        /// <param name="trafficV2ProcessingService">The processing service for traffic
        /// snapshots.</param>
        public HealthTrafficClientV2(ITrafficV2ProcessingService trafficV2ProcessingService) =>
            this.trafficV2ProcessingService = trafficV2ProcessingService;

        /// <summary>
        /// Retrieves a time-bucketed traffic snapshot asynchronously by delegating to the
        /// processing service and handling any exceptions that occur.
        /// </summary>
        /// <param name="period">The period granularity to aggregate over.</param>
        /// <param name="windowStart">The inclusive UTC start of the window, or
        /// <see cref="DateTimeOffset.MinValue"/> for the current period.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation.</param>
        /// <returns>A <see cref="ValueTask{TrafficSnapshotV2}"/> representing the asynchronous
        /// operation that returns the traffic snapshot.</returns>
        public async ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.trafficV2ProcessingService
                    .RetrieveTrafficSnapshotV2Async(period, windowStart, cancellationToken);
            }
            catch (TrafficV2ProcessingDependencyException
                trafficV2ProcessingDependencyException)
            {
                throw CreateHealthTrafficClientV2DependencyException(
                    trafficV2ProcessingDependencyException.InnerException as Xeption);
            }
            catch (TrafficV2ProcessingServiceException
                trafficV2ProcessingServiceException)
            {
                throw CreateHealthTrafficClientV2DependencyException(
                    trafficV2ProcessingServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthTrafficClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthTrafficClientV2DependencyException
            CreateHealthTrafficClientV2DependencyException(Xeption innerException)
        {
            return new HealthTrafficClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthTrafficClientV2ServiceException
            CreateHealthTrafficClientV2ServiceException(Xeption innerException)
        {
            return new HealthTrafficClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
