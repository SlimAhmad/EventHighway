// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health traffic client implementation, handling traffic snapshot
    /// retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthTrafficClientV2 : IHealthTrafficClientV2
    {
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthTrafficClientV2"/> class with the
        /// specified health check coordination service.
        /// </summary>
        /// <param name="healthV2CoordinationService">The coordination service for health
        /// checks.</param>
        public HealthTrafficClientV2(IHealthV2CoordinationService healthV2CoordinationService) =>
            this.healthV2CoordinationService = healthV2CoordinationService;

        /// <summary>
        /// Retrieves a time-bucketed traffic snapshot asynchronously by delegating to the
        /// coordination service and handling any exceptions that occur.
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
                return await this.healthV2CoordinationService
                    .RetrieveTrafficSnapshotV2Async(period, windowStart, cancellationToken);
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthTrafficClientV2ValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthTrafficClientV2ValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthTrafficClientV2DependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthTrafficClientV2DependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateHealthTrafficClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthTrafficClientV2ValidationException
            CreateHealthTrafficClientV2ValidationException(Xeption innerException)
        {
            return new HealthTrafficClientV2ValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
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
