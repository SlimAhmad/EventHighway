// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.RagStatuses.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.RagStatuses.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health check client implementation, handling health check retrieval
    /// operations while managing coordination service exceptions.
    /// </summary>
    internal class HealthStatusClientV2 : IHealthStatusClientV2
    {
        private readonly IRagStatusV2OrchestrationService ragStatusV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthStatusClientV2"/> class with the
        /// specified rag status orchestration service.
        /// </summary>
        /// <param name="ragStatusV2OrchestrationService">The orchestration service for rag status
        /// health checks.</param>
        public HealthStatusClientV2(IRagStatusV2OrchestrationService ragStatusV2OrchestrationService) =>
            this.ragStatusV2OrchestrationService = ragStatusV2OrchestrationService;

        /// <summary>
        /// Retrieves a summary of health check items asynchronously by delegating to the
        /// coordination service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IEnumerable}"/> representing the asynchronous
        /// operation that returns a collection of health check items.</returns>
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
                return await this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(cancellationToken);
            }
            catch (RagStatusV2OrchestrationDependencyException
                ragStatusV2OrchestrationDependencyException)
            {
                throw CreateHealthStatusClientV2DependencyException(
                    ragStatusV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (RagStatusV2OrchestrationServiceException
                ragStatusV2OrchestrationServiceException)
            {
                throw CreateHealthStatusClientV2DependencyException(
                    ragStatusV2OrchestrationServiceException.InnerException as Xeption);
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
