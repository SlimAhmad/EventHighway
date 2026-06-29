// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public async ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthStatusClientV2
                .RetrieveHealthRagStatusV2Async(cancellationToken);

        public async ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthTrafficClientV2
                .RetrieveTrafficSnapshotV2Async(period, windowStart, cancellationToken);

        public async ValueTask<IEnumerable<EventAddressSummaryV2>>
            RetrieveEventAddressSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthAddressClientV2
                .RetrieveEventAddressSummaryV2Async(period, windowStart, cancellationToken);

        public async ValueTask<LoopDetectionSummaryV2> RetrieveLoopDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthLoopClientV2
                .RetrieveLoopDetectionSummaryV2Async(period, windowStart, cancellationToken);

        public async ValueTask<DuplicateDetectionSummaryV2>
            RetrieveDuplicateDetectionSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthDuplicateClientV2
                .RetrieveDuplicateDetectionSummaryV2Async(period, windowStart, cancellationToken);

        public async ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthRetryClientV2
                .RetrieveRetryHealthV2Async(cancellationToken);

        public async ValueTask<IEnumerable<ParticipantSummaryV2>>
            RetrieveParticipantSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthParticipantClientV2
                .RetrieveParticipantSummaryV2Async(period, windowStart, cancellationToken);
    }
}
