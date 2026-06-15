// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Defines the contract for the V2 health check client, providing operations to retrieve
    /// health status information.
    /// </summary>
    public interface IHealthV2Client
    {
        /// <summary>
        /// Retrieves a summary of health check items asynchronously. This operation collects
        /// health status information from various system components.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IEnumerable}"/> representing the asynchronous
        /// operation that returns a collection of health check items containing status
        /// information.</returns>
        /// <exception cref="HealthV2ClientValidationException">Thrown when validation errors
        /// occur during health check retrieval.</exception>
        /// <exception cref="HealthV2ClientDependencyException">Thrown when dependency or
        /// service errors occur during health check retrieval.</exception>
        /// <exception cref="HealthV2ClientServiceException">Thrown when an unexpected error
        /// occurs during health check retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthSummaryV2Async(
            CancellationToken cancellationToken = default);
    }
}
