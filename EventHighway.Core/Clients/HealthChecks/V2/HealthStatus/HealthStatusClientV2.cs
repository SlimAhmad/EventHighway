// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    /// Represents the V2 health check client implementation, handling health check retrieval
    /// operations while managing coordination service exceptions.
    /// </summary>
    internal class HealthStatusClientV2 : IHealthStatusClientV2
    {
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthStatusClientV2"/> class with the
        /// specified health check coordination service.
        /// </summary>
        /// <param name="healthV2CoordinationService">The coordination service for health
        /// checks.</param>
        public HealthStatusClientV2(IHealthV2CoordinationService healthV2CoordinationService) =>
            this.healthV2CoordinationService = healthV2CoordinationService;

        /// <summary>
        /// Retrieves a summary of health check items asynchronously by delegating to the
        /// coordination service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IEnumerable}"/> representing the asynchronous
        /// operation that returns a collection of health check items.</returns>
        /// <exception cref="HealthStatusClientV2ValidationException">Thrown when validation errors
        /// occur in the coordination service.</exception>
        /// <exception cref="HealthStatusClientV2DependencyException">Thrown when dependency or
        /// service errors occur.</exception>
        /// <exception cref="HealthStatusClientV2ServiceException">Thrown when an unexpected error
        /// occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.healthV2CoordinationService
                    .RetrieveHealthRagStatusV2Async(cancellationToken);
            }
            catch (HealthV2CoordinationValidationException
                healthV2CoordinationValidationException)
            {
                throw CreateHealthStatusClientV2ValidationException(
                    healthV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyValidationException
                healthV2CoordinationDependencyValidationException)
            {
                throw CreateHealthStatusClientV2ValidationException(
                    healthV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationDependencyException
                healthV2CoordinationDependencyException)
            {
                throw CreateHealthStatusClientV2DependencyException(
                    healthV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (HealthV2CoordinationServiceException
                healthV2CoordinationServiceException)
            {
                throw CreateHealthStatusClientV2DependencyException(
                    healthV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthStatusClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthStatusClientV2ValidationException
            CreateHealthStatusClientV2ValidationException(Xeption innerException)
        {
            return new HealthStatusClientV2ValidationException(
                message: "Health client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthStatusClientV2DependencyException
            CreateHealthStatusClientV2DependencyException(Xeption innerException)
        {
            return new HealthStatusClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthStatusClientV2ServiceException
            CreateHealthStatusClientV2ServiceException(Xeption innerException)
        {
            return new HealthStatusClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
