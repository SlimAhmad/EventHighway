// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;

namespace EventHighway.Portal.Web.Services.Views.HealthDashboards
{
    public interface IHealthViewService
    {
        ValueTask<List<HealthRagTile>> RetrieveHealthRagTilesAsync(
            CancellationToken cancellationToken = default);

        ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default);

        ValueTask<List<EventAddressSummaryV2>> RetrieveAddressSummariesAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default);

        ValueTask<LoopDetectionSummaryV2> RetrieveLoopSummaryAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default);

        ValueTask<DuplicateDetectionSummaryV2> RetrieveDuplicateSummaryAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default);

        ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthAsync(
            CancellationToken cancellationToken = default);

        ValueTask<List<ParticipantSummaryV2>> RetrieveParticipantSummariesAsync(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default);
    }
}
