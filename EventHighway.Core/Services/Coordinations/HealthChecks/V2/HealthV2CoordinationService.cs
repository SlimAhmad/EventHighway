// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;

namespace EventHighway.Core.Services.Coordinations.HealthChecks.V2
{
    internal partial class HealthV2CoordinationService : IHealthV2CoordinationService
    {
        private readonly IEventV2OrchestrationService eventV2OrchestrationService;
        private readonly IEventListenerV2OrchestrationService eventListenerV2OrchestrationService;
        private readonly IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public HealthV2CoordinationService(
            IEventV2OrchestrationService eventV2OrchestrationService,
            IEventListenerV2OrchestrationService eventListenerV2OrchestrationService,
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2OrchestrationService = eventV2OrchestrationService;
            this.eventListenerV2OrchestrationService = eventListenerV2OrchestrationService;
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthSummaryV2Async(
            CancellationToken cancellationToken = default) =>
        TryCatch<IEnumerable<HealthCheckItemV2>>(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allEvents =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            var allAddresses =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventAddressV2sAsync(cancellationToken);

            var allListeners =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllEventListenerV2sAsync(cancellationToken);

            var allListenerEvents =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            var allHandlers =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllEventHandlerV2sAsync(cancellationToken);

            var allArchivedEvents =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken);

            var allArchivedListenerEvents =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);

            int totalAddresses = allAddresses.Count();
            int totalListeners = allListeners.Count();
            int totalEvents = allEvents.Count();
            int immediateEvents = allEvents.Count(e => e.Type == EventTypeV2.Immediate);
            int scheduledEvents = allEvents.Count(e => e.Type == EventTypeV2.Scheduled);
            int deadEvents = allEvents.Count(e => e.RemainingRetryAttempts == 0);
            int totalListenerEvents = allListenerEvents.Count();
            int pendingListenerEvents = allListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Pending);
            int successListenerEvents = allListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Success);
            int errorListenerEvents = allListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error);

            decimal errorRate = totalListenerEvents > 0
                ? (decimal)errorListenerEvents / totalListenerEvents * 100
                : 0;

            int totalArchivedEvents = allArchivedEvents.Count();
            int totalArchivedListenerEvents = allArchivedListenerEvents.Count();

            int archivedListenerErrors =
                allArchivedListenerEvents.Count(la =>
                    la.Status == ListenerEventArchiveStatusV2.Error);

            int handlerCount = allHandlers.Count();
            int quarantinedEvents = allEvents.Count(e => e.Status == EventStatusV2.Quarantined);

            int quarantinedArchives =
                allArchivedEvents.Count(a => a.Status == EventArchiveStatusV2.Quarantined);

            HealthConfiguration healthConfig =
                this.configurationBroker.GetHealthConfiguration();

            HealthStatusV2 deadEventsStatus =
                ComputeRagStatus(deadEvents, HealthMetric.DeadEvents, healthConfig);

            HealthStatusV2 errorRateStatus =
                ComputeRagStatus(errorRate, HealthMetric.ErrorRate, healthConfig);

            HealthStatusV2 handlerStatus =
                ComputeRagStatus(handlerCount, HealthMetric.HandlerCount, healthConfig);

            HealthStatusV2 loopsDetectedStatus =
                ComputeRagStatus(quarantinedEvents, HealthMetric.LoopsDetected, healthConfig);

            return new List<HealthCheckItemV2>
            {
                CreateItem("Event Addresses", "Total", totalAddresses.ToString(), HealthStatusV2.NA),
                CreateItem("Event Listeners", "Total", totalListeners.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Total", totalEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Immediate", immediateEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Scheduled", scheduledEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Active Events", "Dead (0 retries)", deadEvents.ToString(), deadEventsStatus),
                CreateItem("Listener Events", "Total", totalListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Pending", pendingListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Successful", successListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Errors", errorListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Listener Events", "Error Rate %", $"{errorRate:F2}", errorRateStatus),
                CreateItem("Event Archives", "Total Archived Events", totalArchivedEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Event Archives", "Total Archived Listener Events", totalArchivedListenerEvents.ToString(), HealthStatusV2.NA),
                CreateItem("Event Archives", "Archived Listener Errors", archivedListenerErrors.ToString(), HealthStatusV2.NA),
                CreateItem("Event Handlers", "Registered Handlers", handlerCount.ToString(), handlerStatus),
                CreateItem("Loop Detection", "Quarantined Events", quarantinedEvents.ToString(), loopsDetectedStatus),
                CreateItem("Loop Detection", "Quarantined Archives", quarantinedArchives.ToString(), HealthStatusV2.NA),
            };
        });

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
                await this.eventV2OrchestrationService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            var allListenerEvents =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

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

        public ValueTask<IEnumerable<EventAddressSummaryV2>> RetrieveEventAddressSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch<IEnumerable<EventAddressSummaryV2>>(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset effectiveWindowStart = ResolveWindowStart(windowStart);
            DateTimeOffset windowEnd = ComputeWindowEnd(period, effectiveWindowStart);
            string windowLabel = BuildWindowLabel(period, effectiveWindowStart, windowEnd);

            var allAddresses =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventAddressV2sAsync(cancellationToken);

            var allEvents =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            var allListeners =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllEventListenerV2sAsync(cancellationToken);

            var allListenerEvents =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            var allArchivedEvents =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken);

            var allArchivedListenerEvents =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);

            HealthConfiguration healthConfig =
                this.configurationBroker.GetHealthConfiguration();

            var summaries = new List<EventAddressSummaryV2>();

            foreach (var address in allAddresses)
            {
                var addressEvents = allEvents
                    .Where(e => e.EventAddressId == address.Id
                        && e.CreatedDate >= effectiveWindowStart && e.CreatedDate < windowEnd)
                    .ToList();

                var addressListenerEvents = allListenerEvents
                    .Where(le => le.EventAddressId == address.Id
                        && le.CreatedDate >= effectiveWindowStart && le.CreatedDate < windowEnd)
                    .ToList();

                int totalArchivedEvents = allArchivedEvents.Count(a =>
                    a.EventAddressId == address.Id
                    && a.ArchivedDate >= effectiveWindowStart && a.ArchivedDate < windowEnd);

                int totalArchivedListenerEvents = allArchivedListenerEvents.Count(la =>
                    la.EventAddressId == address.Id
                    && la.ArchivedDate >= effectiveWindowStart && la.ArchivedDate < windowEnd);

                int activeListeners = allListeners.Count(l => l.EventAddressId == address.Id);

                int totalListenerEvents = addressListenerEvents.Count;
                int errorListenerEvents = addressListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error);

                decimal errorRate = totalListenerEvents > 0
                    ? (decimal)errorListenerEvents / totalListenerEvents * 100
                    : 0;

                int totalActiveEvents = addressEvents.Count;
                int distinctContentHashes = addressEvents.Select(e => e.ContentHash).Distinct().Count();
                int duplicates = totalActiveEvents - distinctContentHashes;

                decimal duplicateRate = totalActiveEvents > 0
                    ? (decimal)duplicates / totalActiveEvents * 100
                    : 0;

                var activityDates = addressEvents.Select(e => e.CreatedDate)
                    .Concat(addressListenerEvents.Select(le => le.CreatedDate))
                    .ToList();

                DateTimeOffset? lastActivity = activityDates.Count > 0
                    ? activityDates.Max()
                    : (DateTimeOffset?)null;

                summaries.Add(new EventAddressSummaryV2
                {
                    Id = address.Id,
                    Name = address.Name,
                    Description = address.Description,
                    Period = period,
                    WindowStart = effectiveWindowStart,
                    WindowEnd = windowEnd,
                    WindowLabel = windowLabel,
                    TotalActiveEvents = totalActiveEvents,
                    TotalArchivedEvents = totalArchivedEvents,
                    TotalListenerEvents = totalListenerEvents,
                    TotalArchivedListenerEvents = totalArchivedListenerEvents,
                    ActiveListeners = activeListeners,
                    DeadEvents = addressEvents.Count(e => e.RemainingRetryAttempts == 0),
                    LoopsDetected = addressEvents.Count(e => e.Status == EventStatusV2.Quarantined),
                    ErrorRate = errorRate,
                    DuplicateRate = duplicateRate,
                    Status = ComputeRagStatus(errorRate, HealthMetric.ErrorRate, healthConfig),
                    LastActivity = lastActivity
                });
            }

            return summaries;
        });

        public ValueTask<LoopDetectionSummaryV2> RetrieveLoopDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset effectiveWindowStart = ResolveWindowStart(windowStart);
            DateTimeOffset windowEnd = ComputeWindowEnd(period, effectiveWindowStart);
            string windowLabel = BuildWindowLabel(period, effectiveWindowStart, windowEnd);

            var allAddresses =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventAddressV2sAsync(cancellationToken);

            var allEvents =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            var allArchivedEvents =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken);

            HealthConfiguration healthConfig =
                this.configurationBroker.GetHealthConfiguration();

            var quarantinedEvents = allEvents
                .Where(e => e.Status == EventStatusV2.Quarantined
                    && e.CreatedDate >= effectiveWindowStart && e.CreatedDate < windowEnd)
                .ToList();

            var quarantinedArchives = allArchivedEvents
                .Where(a => a.Status == EventArchiveStatusV2.Quarantined
                    && a.ArchivedDate >= effectiveWindowStart && a.ArchivedDate < windowEnd)
                .ToList();

            var addressNames = allAddresses
                .ToDictionary(address => address.Id, address => address.Name);

            var addressIds = quarantinedEvents.Select(e => e.EventAddressId)
                .Concat(quarantinedArchives.Select(a => a.EventAddressId))
                .Distinct()
                .ToList();

            var byAddress = new List<LoopDetailV2>();

            foreach (var addressId in addressIds)
            {
                var addressActive = quarantinedEvents
                    .Where(e => e.EventAddressId == addressId)
                    .ToList();

                var addressArchived = quarantinedArchives
                    .Where(a => a.EventAddressId == addressId)
                    .ToList();

                var detectionDates = addressActive.Select(e => e.CreatedDate)
                    .Concat(addressArchived.Select(a => a.ArchivedDate))
                    .ToList();

                DateTimeOffset? mostRecentDetection = detectionDates.Count > 0
                    ? detectionDates.Max()
                    : (DateTimeOffset?)null;

                byAddress.Add(new LoopDetailV2
                {
                    EventAddressId = addressId,
                    AddressName = addressNames.TryGetValue(addressId, out string name) ? name : null,
                    ActiveQuarantined = addressActive.Count,
                    ArchivedQuarantined = addressArchived.Count,
                    InWindow = addressActive.Count + addressArchived.Count,
                    MostRecentDetection = mostRecentDetection,
                    Status = ComputeRagStatus(
                        addressActive.Count, HealthMetric.LoopsDetected, healthConfig)
                });
            }

            return new LoopDetectionSummaryV2
            {
                Period = period,
                WindowStart = effectiveWindowStart,
                WindowEnd = windowEnd,
                WindowLabel = windowLabel,
                TotalActiveQuarantined = quarantinedEvents.Count,
                TotalArchivedQuarantined = quarantinedArchives.Count,
                TotalInWindow = quarantinedEvents.Count + quarantinedArchives.Count,
                ByAddress = byAddress
                    .OrderByDescending(detail => detail.InWindow)
                    .ToList()
            };
        });

        public ValueTask<DuplicateDetectionSummaryV2> RetrieveDuplicateDetectionSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset effectiveWindowStart = ResolveWindowStart(windowStart);
            DateTimeOffset windowEnd = ComputeWindowEnd(period, effectiveWindowStart);
            string windowLabel = BuildWindowLabel(period, effectiveWindowStart, windowEnd);

            var allAddresses =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventAddressV2sAsync(cancellationToken);

            var allEvents =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            var windowEvents = allEvents
                .Where(e => e.CreatedDate >= effectiveWindowStart && e.CreatedDate < windowEnd)
                .ToList();

            var byAddress = new List<DuplicateDetailV2>();

            foreach (var address in allAddresses)
            {
                var addressEvents = windowEvents
                    .Where(e => e.EventAddressId == address.Id)
                    .ToList();

                if (addressEvents.Count == 0)
                {
                    continue;
                }

                var contentHashGroups = addressEvents
                    .GroupBy(e => e.ContentHash)
                    .ToList();

                int totalEvents = addressEvents.Count;
                int distinctContentHashes = contentHashGroups.Count;
                int duplicates = totalEvents - distinctContentHashes;

                var duplicateEvents = contentHashGroups
                    .Where(group => group.Count() > 1)
                    .SelectMany(group => group.OrderBy(e => e.CreatedDate).Skip(1))
                    .ToList();

                DateTimeOffset? lastDuplicateSeen = duplicateEvents.Count > 0
                    ? duplicateEvents.Max(e => e.CreatedDate)
                    : (DateTimeOffset?)null;

                decimal duplicateRate = totalEvents > 0
                    ? (decimal)duplicates / totalEvents * 100
                    : 0;

                byAddress.Add(new DuplicateDetailV2
                {
                    EventAddressId = address.Id,
                    AddressName = address.Name,
                    ParticipantId = null,
                    ParticipantName = "Unknown",
                    TotalEvents = totalEvents,
                    Duplicates = duplicates,
                    DuplicateRate = duplicateRate,
                    LastDuplicateSeen = lastDuplicateSeen
                });
            }

            long totalDuplicatesDetected = byAddress.Sum(detail => detail.Duplicates);
            long totalEventsInWindow = byAddress.Sum(detail => detail.TotalEvents);
            long totalUniqueEvents = totalEventsInWindow - totalDuplicatesDetected;

            decimal overallDuplicateRate = totalEventsInWindow > 0
                ? (decimal)totalDuplicatesDetected / totalEventsInWindow * 100
                : 0;

            return new DuplicateDetectionSummaryV2
            {
                Period = period,
                WindowStart = effectiveWindowStart,
                WindowEnd = windowEnd,
                WindowLabel = windowLabel,
                TotalDuplicatesDetected = totalDuplicatesDetected,
                TotalUniqueEvents = totalUniqueEvents,
                OverallDuplicateRate = overallDuplicateRate,
                ByAddress = byAddress
                    .OrderByDescending(detail => detail.Duplicates)
                    .ToList()
            };
        });

        public ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<IEnumerable<ParticipantSummaryV2>> RetrieveParticipantSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        private static HealthStatusV2 ComputeRagStatus(
            decimal value,
            HealthMetric metric,
            HealthConfiguration healthConfig)
        {
            RagThreshold threshold =
                healthConfig.Thresholds.FirstOrDefault(t => t.Metric == metric);

            if (threshold is null)
                return HealthStatusV2.NA;

            if (threshold.Green < threshold.Red)
            {
                if (value <= threshold.Green) return HealthStatusV2.Green;
                if (value >= threshold.Red) return HealthStatusV2.Red;
                return HealthStatusV2.Amber;
            }

            if (threshold.Green > threshold.Red)
            {
                if (value >= threshold.Green) return HealthStatusV2.Green;
                if (value <= threshold.Red) return HealthStatusV2.Red;
                return HealthStatusV2.Amber;
            }

            return HealthStatusV2.NA;
        }

        private static HealthCheckItemV2 CreateItem(
            string grouping,
            string item,
            string value,
            HealthStatusV2 status)
        {
            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value,
                StatusCode = (int)status,
                Status = status.ToString()
            };
        }

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
