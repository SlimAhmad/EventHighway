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
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.ParticipantSummaries.V2
{
    internal partial class ParticipantSummaryV2OrchestrationService : IParticipantSummaryV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventV2Service eventV2Service;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public ParticipantSummaryV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventV2Service eventV2Service,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventV2Service = eventV2Service;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<ParticipantSummaryV2>> RetrieveParticipantSummaryV2Async(
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

            var allListeners = allAddresses
                .SelectMany(address =>
                    address.EventListenerV2s ?? Enumerable.Empty<EventListenerV2>())
                .ToList();

            var allListenerEvents = allEvents
                .SelectMany(eventV2 =>
                    eventV2.ListenerEventV2s ?? Enumerable.Empty<ListenerEventV2>())
                .ToList();

            HealthConfiguration healthConfig =
                this.configurationBroker.GetHealthConfiguration();

            var windowEvents = allEvents
                .Where(e => e.CreatedDate >= effectiveWindowStart && e.CreatedDate < windowEnd)
                .ToList();

            var windowListenerEvents = allListenerEvents
                .Where(le => le.CreatedDate >= effectiveWindowStart && le.CreatedDate < windowEnd)
                .ToList();

            var addressNames = allAddresses
                .ToDictionary(address => address.Id, address => address.Name);

            var participantsById = windowEvents
                .Where(e => e.EventParticipantV2 != null).Select(e => e.EventParticipantV2)
                .Concat(allListeners.Where(l => l.EventParticipantV2 != null).Select(l => l.EventParticipantV2))
                .GroupBy(participant => participant.Id)
                .ToDictionary(group => group.Key, group => group.First());

            var participantKeys = windowEvents.Select(e => e.EventParticipantV2Id ?? Guid.Empty)
                .Concat(allListeners.Select(l => l.EventParticipantV2Id ?? Guid.Empty))
                .Distinct()
                .ToList();

            var summaries = new List<ParticipantSummaryV2>();

            foreach (var participantKey in participantKeys)
            {
                var participantEvents = windowEvents
                    .Where(e => (e.EventParticipantV2Id ?? Guid.Empty) == participantKey)
                    .ToList();

                var ownedListenerIds = allListeners
                    .Where(l => (l.EventParticipantV2Id ?? Guid.Empty) == participantKey)
                    .Select(l => l.Id)
                    .ToHashSet();

                var participantEventIds = participantEvents.Select(e => e.Id).ToHashSet();

                var ownedListenerEvents = windowListenerEvents
                    .Where(le => ownedListenerIds.Contains(le.EventListenerV2Id))
                    .ToList();

                var publisherListenerEvents = windowListenerEvents
                    .Where(le => participantEventIds.Contains(le.EventV2Id))
                    .ToList();

                decimal publisherErrorRate = publisherListenerEvents.Count > 0
                    ? (decimal)publisherListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error)
                        / publisherListenerEvents.Count * 100
                    : 0;

                decimal listenerErrorRate = ownedListenerEvents.Count > 0
                    ? (decimal)ownedListenerEvents.Count(le => le.Status == ListenerEventStatusV2.Error)
                        / ownedListenerEvents.Count * 100
                    : 0;

                var addressIds = participantEvents
                    .Select(e => e.EventAddressV2Id)
                    .Distinct()
                    .ToList();

                var addressNamesForParticipant = addressIds
                    .Where(id => addressNames.ContainsKey(id))
                    .Select(id => addressNames[id])
                    .ToList();

                int distinctContentHashes = participantEvents.Select(e => e.ContentHash).Distinct().Count();

                var activityDates = participantEvents.Select(e => e.CreatedDate)
                    .Concat(ownedListenerEvents.Select(le => le.CreatedDate))
                    .ToList();

                DateTimeOffset? lastActivity = activityDates.Count > 0
                    ? activityDates.Max()
                    : (DateTimeOffset?)null;

                participantsById.TryGetValue(participantKey, out var participant);

                bool isKnownParticipant =
                    participantKey != Guid.Empty && participant != null;

                summaries.Add(new ParticipantSummaryV2
                {
                    EventParticipantV2Id = participantKey,
                    Name = isKnownParticipant ? participant.Name : "Unknown",
                    ContactEmail = isKnownParticipant ? participant.ContactEmail : null,
                    ContactPhone = isKnownParticipant ? participant.ContactPhone : null,
                    IsActive = isKnownParticipant && participant.IsActive,
                    Period = period,
                    WindowStart = effectiveWindowStart,
                    WindowEnd = windowEnd,
                    WindowLabel = windowLabel,
                    TotalEventsSubmitted = participantEvents.Count,
                    ActiveEventAddresses = addressIds.Count,
                    ActiveEventAddressNames = addressNamesForParticipant,
                    TotalListenerEvents = ownedListenerEvents.Count,
                    OwnedListeners = ownedListenerIds.Count,
                    PublisherErrorRate = publisherErrorRate,
                    ListenerErrorRate = listenerErrorRate,
                    LoopsDetected = participantEvents.Count(e => e.Status == EventStatusV2.Quarantined),
                    DuplicatesDetected = participantEvents.Count - distinctContentHashes,
                    Status = ComputeRagStatus(publisherErrorRate, HealthMetric.ErrorRate, healthConfig),
                    LastActivity = lastActivity
                });
            }

            return summaries
                .OrderByDescending(summary => summary.TotalEventsSubmitted)
                .ThenBy(summary => summary.Name)
                .ToList();
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
