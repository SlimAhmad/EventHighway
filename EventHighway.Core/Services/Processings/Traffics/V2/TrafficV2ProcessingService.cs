// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Processings.Traffics.V2
{
    internal partial class TrafficV2ProcessingService : ITrafficV2ProcessingService
    {
        private readonly IEventV2Service eventV2Service;
        private readonly ILoggingBroker loggingBroker;

        public TrafficV2ProcessingService(
            IEventV2Service eventV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventV2Service = eventV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            DateTimeOffset effectiveWindowStart = ResolveWindowStart(windowStart);
            DateTimeOffset windowEnd = ComputeWindowEnd(period, effectiveWindowStart);
            string windowLabel = BuildWindowLabel(period, effectiveWindowStart, windowEnd);

            var allEvents =
                (await this.eventV2Service
                    .RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                        .ToList();

            var allListenerEvents = allEvents
                .SelectMany(eventV2 =>
                    eventV2.ListenerEventV2s ?? Enumerable.Empty<ListenerEventV2>())
                .ToList();

            var windowEvents = allEvents
                .Where(e => e.CreatedDate >= effectiveWindowStart && e.CreatedDate < windowEnd)
                .ToList();

            var windowListenerEvents = allListenerEvents
                .Where(le => le.CreatedDate >= effectiveWindowStart && le.CreatedDate < windowEnd)
                .ToList();

            return new TrafficSnapshotV2
            {
                Period = period,
                WindowStart = effectiveWindowStart,
                WindowEnd = windowEnd,
                WindowLabel = windowLabel,
                TotalEvents = windowEvents.Count,
                TotalListenerEvents = windowListenerEvents.Count,
                TotalSuccess = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Success),
                TotalErrors = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error),
                TotalPending = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Pending),
                TotalReplays = windowListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Replay),
                Buckets = BuildTrafficBuckets(
                    period, effectiveWindowStart, windowEnd, windowEvents, windowListenerEvents)
            };
        });

        private static DateTimeOffset ResolveWindowStart(DateTimeOffset windowStart) =>
            windowStart == DateTimeOffset.MinValue
                ? DateTimeOffset.UtcNow
                : windowStart;

        private static DateTimeOffset ComputeWindowEnd(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart)
        {
            switch (period)
            {
                case TrafficPeriodV2.Day:
                    return windowStart.AddHours(24);

                case TrafficPeriodV2.Week:
                    return windowStart.AddDays(7);

                case TrafficPeriodV2.Month:
                    return new DateTimeOffset(
                        windowStart.Year, windowStart.Month, 1, 0, 0, 0, TimeSpan.Zero)
                            .AddMonths(1);

                case TrafficPeriodV2.Year:
                    return new DateTimeOffset(
                        windowStart.Year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);

                default:
                    return windowStart.AddHours(24);
            }
        }

        private static string BuildWindowLabel(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd)
        {
            switch (period)
            {
                case TrafficPeriodV2.Day:
                    return windowStart.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);

                case TrafficPeriodV2.Week:
                    return $"{windowStart.ToString("dd MMM", CultureInfo.InvariantCulture)} – " +
                        $"{windowEnd.AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}";

                case TrafficPeriodV2.Month:
                    return windowStart.ToString("MMM yyyy", CultureInfo.InvariantCulture);

                case TrafficPeriodV2.Year:
                    return windowStart.Year.ToString(CultureInfo.InvariantCulture);

                default:
                    return windowStart.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
            }
        }

        private static IEnumerable<TrafficBucketV2> BuildTrafficBuckets(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            DateTimeOffset windowEnd,
            IEnumerable<EventV2> windowEvents,
            IEnumerable<ListenerEventV2> windowListenerEvents)
        {
            var bucketStarts = new List<(DateTimeOffset Start, DateTimeOffset End, string Label)>();

            switch (period)
            {
                case TrafficPeriodV2.Day:
                    for (int i = 0; i < 24; i++)
                    {
                        DateTimeOffset start = windowStart.AddHours(i);
                        bucketStarts.Add((start, start.AddHours(1),
                            start.ToString("HH:00", CultureInfo.InvariantCulture)));
                    }

                    break;

                case TrafficPeriodV2.Week:
                    for (int i = 0; i < 7; i++)
                    {
                        DateTimeOffset start = windowStart.AddDays(i);
                        bucketStarts.Add((start, start.AddDays(1),
                            start.ToString("ddd", CultureInfo.InvariantCulture)));
                    }

                    break;

                case TrafficPeriodV2.Month:
                    for (DateTimeOffset start = windowStart; start < windowEnd; start = start.AddDays(1))
                    {
                        bucketStarts.Add((start, start.AddDays(1),
                            start.ToString("dd", CultureInfo.InvariantCulture)));
                    }

                    break;

                case TrafficPeriodV2.Year:
                    DateTimeOffset firstOfYear =
                        new DateTimeOffset(windowStart.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);

                    for (int i = 0; i < 12; i++)
                    {
                        DateTimeOffset start = firstOfYear.AddMonths(i);
                        bucketStarts.Add((start, start.AddMonths(1),
                            start.ToString("MMM", CultureInfo.InvariantCulture)));
                    }

                    break;
            }

            return bucketStarts.Select(bucket => new TrafficBucketV2
            {
                PeriodStart = bucket.Start,
                Label = bucket.Label,
                Events = windowEvents.Count(e => e.CreatedDate >= bucket.Start && e.CreatedDate < bucket.End),
                ImmediateEvents = windowEvents.Count(e =>
                    e.CreatedDate >= bucket.Start && e.CreatedDate < bucket.End
                    && e.Type == EventTypeV2.Immediate),
                ScheduledEvents = windowEvents.Count(e =>
                    e.CreatedDate >= bucket.Start && e.CreatedDate < bucket.End
                    && e.Type == EventTypeV2.Scheduled),
                ListenerEvents = windowListenerEvents.Count(le =>
                    le.CreatedDate >= bucket.Start && le.CreatedDate < bucket.End),
                Success = windowListenerEvents.Count(le =>
                    le.CreatedDate >= bucket.Start && le.CreatedDate < bucket.End
                    && le.Status == ListenerEventStatusV2.Success),
                Errors = windowListenerEvents.Count(le =>
                    le.CreatedDate >= bucket.Start && le.CreatedDate < bucket.End
                    && le.Status == ListenerEventStatusV2.Error),
                Pending = windowListenerEvents.Count(le =>
                    le.CreatedDate >= bucket.Start && le.CreatedDate < bucket.End
                    && le.Status == ListenerEventStatusV2.Pending),
                Replays = windowListenerEvents.Count(le =>
                    le.CreatedDate >= bucket.Start && le.CreatedDate < bucket.End
                    && le.Status == ListenerEventStatusV2.Replay)
            }).ToList();
        }
    }
}
