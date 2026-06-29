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
        public ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthStatusClientV2
                    .RetrieveHealthRagStatusV2Async(cancellationToken),
                cancellationToken);

        public ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthTrafficClientV2
                    .RetrieveTrafficSnapshotV2Async(period, windowStart, cancellationToken),
                cancellationToken);

        public ValueTask<IEnumerable<EventAddressSummaryV2>>
            RetrieveEventAddressSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthAddressClientV2
                    .RetrieveEventAddressSummaryV2Async(period, windowStart, cancellationToken),
                cancellationToken);

        public ValueTask<LoopDetectionSummaryV2> RetrieveLoopDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthLoopClientV2
                    .RetrieveLoopDetectionSummaryV2Async(period, windowStart, cancellationToken),
                cancellationToken);

        public ValueTask<DuplicateDetectionSummaryV2>
            RetrieveDuplicateDetectionSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthDuplicateClientV2
                    .RetrieveDuplicateDetectionSummaryV2Async(period, windowStart, cancellationToken),
                cancellationToken);

        public ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthRetryClientV2
                    .RetrieveRetryHealthV2Async(cancellationToken),
                cancellationToken);

        public ValueTask<IEnumerable<ParticipantSummaryV2>>
            RetrieveParticipantSummaryV2Async(
                TrafficPeriodV2 period,
                DateTimeOffset windowStart,
                CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.HealthParticipantClientV2
                    .RetrieveParticipantSummaryV2Async(period, windowStart, cancellationToken),
                cancellationToken);
    }
}
