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
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.LoopDetections.V2
{
    internal partial class LoopDetectionV2OrchestrationService : ILoopDetectionV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventV2Service eventV2Service;
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public LoopDetectionV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventV2Service eventV2Service,
            IEventArchiveV2Service eventArchiveV2Service,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventV2Service = eventV2Service;
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

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
                (await this.eventAddressV2Service
                    .RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                        .ToList();

            var allEvents =
                (await this.eventV2Service
                    .RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                        .ToList();

            var allArchivedEvents =
                (await this.eventArchiveV2Service
                    .RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(cancellationToken))
                        .ToList();

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

            var addressIds = quarantinedEvents.Select(e => e.EventAddressV2Id)
                .Concat(quarantinedArchives.Select(a => a.EventAddressV2Id))
                .Distinct()
                .ToList();

            var byAddress = new List<LoopDetailV2>();

            foreach (var addressId in addressIds)
            {
                var addressActive = quarantinedEvents
                    .Where(e => e.EventAddressV2Id == addressId)
                    .ToList();

                var addressArchived = quarantinedArchives
                    .Where(a => a.EventAddressV2Id == addressId)
                    .ToList();

                string addressName =
                    addressNames.TryGetValue(addressId, out string name) ? name : null;

                var participantKeys = addressActive.Select(e => e.EventParticipantV2Id ?? Guid.Empty)
                    .Concat(addressArchived.Select(a => a.EventParticipantV2Id ?? Guid.Empty))
                    .Distinct()
                    .ToList();

                foreach (var participantKey in participantKeys)
                {
                    var participantActive = addressActive
                        .Where(e => (e.EventParticipantV2Id ?? Guid.Empty) == participantKey)
                        .ToList();

                    var participantArchived = addressArchived
                        .Where(a => (a.EventParticipantV2Id ?? Guid.Empty) == participantKey)
                        .ToList();

                    var detectionDates = participantActive.Select(e => e.CreatedDate)
                        .Concat(participantArchived.Select(a => a.ArchivedDate))
                        .ToList();

                    DateTimeOffset? mostRecentDetection = detectionDates.Count > 0
                        ? detectionDates.Max()
                        : (DateTimeOffset?)null;

                    var participant = participantActive.Select(e => e.EventParticipantV2)
                        .Concat(participantArchived.Select(a => a.EventParticipantV2))
                        .FirstOrDefault(p => p != null);

                    bool isKnownParticipant =
                        participantKey != Guid.Empty && participant != null;

                    byAddress.Add(new LoopDetailV2
                    {
                        EventAddressV2Id = addressId,
                        EventAddressV2Name = addressName,
                        EventParticipantV2Id = isKnownParticipant ? participantKey : (Guid?)null,
                        EventParticipantV2Name = isKnownParticipant ? participant.Name : "Unknown",
                        ActiveQuarantined = participantActive.Count,
                        ArchivedQuarantined = participantArchived.Count,
                        InWindow = participantActive.Count + participantArchived.Count,
                        MostRecentDetection = mostRecentDetection,
                        Status = ComputeRagStatus(
                            participantActive.Count, HealthMetric.LoopsDetected, healthConfig)
                    });
                }
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
    }
}
