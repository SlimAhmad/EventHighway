// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.ParticipantSummaries.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.ParticipantSummaries.V2;
using Xeptions;

namespace EventHighway.Core.Clients.HealthChecks.V2
{
    /// <summary>
    /// Represents the V2 health participant client implementation, handling per-participant
    /// health-summary retrieval while managing coordination service exceptions.
    /// </summary>
    internal class HealthParticipantClientV2 : IHealthParticipantClientV2
    {
        private readonly IParticipantSummaryV2OrchestrationService participantSummaryV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthParticipantClientV2"/> class with
        /// the specified participant summary orchestration service.
        /// </summary>
        /// <param name="participantSummaryV2OrchestrationService">The orchestration service for
        /// per-participant summaries.</param>
        public HealthParticipantClientV2(IParticipantSummaryV2OrchestrationService participantSummaryV2OrchestrationService) =>
            this.participantSummaryV2OrchestrationService = participantSummaryV2OrchestrationService;

        /// <summary>
        /// Retrieves per-participant health summaries asynchronously by delegating to the
        /// coordination service and handling any exceptions that occur.
        /// </summary>
        /// <param name="period">The traffic period to summarize.</param>
        /// <param name="windowStart">The start of the time window to summarize.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IEnumerable}"/> representing the asynchronous
        /// operation that returns a collection of participant summaries.</returns>
        /// <exception cref="HealthParticipantClientV2DependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="HealthParticipantClientV2ServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IEnumerable<ParticipantSummaryV2>> RetrieveParticipantSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.participantSummaryV2OrchestrationService
                    .RetrieveParticipantSummaryV2Async(period, windowStart, cancellationToken);
            }
            catch (ParticipantSummaryV2OrchestrationDependencyException
                participantSummaryV2OrchestrationDependencyException)
            {
                throw CreateHealthParticipantClientV2DependencyException(
                    participantSummaryV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (ParticipantSummaryV2OrchestrationServiceException
                participantSummaryV2OrchestrationServiceException)
            {
                throw CreateHealthParticipantClientV2DependencyException(
                    participantSummaryV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateHealthParticipantClientV2ServiceException(exception as Xeption);
            }
        }

        private static HealthParticipantClientV2DependencyException
            CreateHealthParticipantClientV2DependencyException(Xeption innerException)
        {
            return new HealthParticipantClientV2DependencyException(
                message: "Health client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static HealthParticipantClientV2ServiceException
            CreateHealthParticipantClientV2ServiceException(Xeption innerException)
        {
            return new HealthParticipantClientV2ServiceException(
                message: "Health client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
