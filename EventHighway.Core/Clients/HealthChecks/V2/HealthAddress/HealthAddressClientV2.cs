// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.AddressSummaries.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.AddressSummaries.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health address client implementation, handling per-event-address
    /// summary retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthAddressClientV2 : IHealthAddressClientV2
    {
        private readonly IAddressSummaryV2OrchestrationService addressSummaryV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthAddressClientV2"/> class with the
        /// specified address summary orchestration service.
        /// </summary>
        /// <param name="addressSummaryV2OrchestrationService">The orchestration service for
        /// per-event-address summaries.</param>
        public HealthAddressClientV2(IAddressSummaryV2OrchestrationService addressSummaryV2OrchestrationService) =>
            this.addressSummaryV2OrchestrationService = addressSummaryV2OrchestrationService;

        /// <summary>
        /// Retrieves a per-event-address health summary asynchronously by delegating to the
        /// coordination service and handling any exceptions that occur.
        /// </summary>
        /// <param name="period">The period granularity to aggregate over.</param>
        /// <param name="windowStart">The inclusive UTC start of the window, or
        /// <see cref="DateTimeOffset.MinValue"/> for the current period.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation.</param>
        /// <returns>A <see cref="ValueTask{IEnumerable}"/> representing the asynchronous
        /// operation that returns one summary per event address.</returns>
        public async ValueTask<IEnumerable<EventAddressSummaryV2>> RetrieveEventAddressSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.addressSummaryV2OrchestrationService
                    .RetrieveEventAddressSummaryV2Async(period, windowStart, cancellationToken);
            }
            catch (AddressSummaryV2OrchestrationDependencyException
                addressSummaryV2OrchestrationDependencyException)
            {
                throw CreateHealthAddressClientV2DependencyException(
                    addressSummaryV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (AddressSummaryV2OrchestrationServiceException
                addressSummaryV2OrchestrationServiceException)
            {
                throw CreateHealthAddressClientV2DependencyException(
                    addressSummaryV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthAddressClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthAddressClientV2DependencyException
            CreateHealthAddressClientV2DependencyException(Xeption innerException)
        {
            return new HealthAddressClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthAddressClientV2ServiceException
            CreateHealthAddressClientV2ServiceException(Xeption innerException)
        {
            return new HealthAddressClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
