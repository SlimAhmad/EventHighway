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
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.DuplicateSummaries.V2
{
    internal partial class DuplicateSummaryV2OrchestrationService : IDuplicateSummaryV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventV2Service eventV2Service;
        private readonly ILoggingBroker loggingBroker;

        public DuplicateSummaryV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventV2Service eventV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventV2Service = eventV2Service;
            this.loggingBroker = loggingBroker;
        }

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
                (await this.eventAddressV2Service
                    .RetrieveAllEventAddressV2sWithEventListenerV2sAsync(cancellationToken))
                        .ToList();

            var allEvents =
                (await this.eventV2Service
                    .RetrieveAllEventV2sWithListenerEventV2sAsync(cancellationToken))
                        .ToList();

            var windowEvents = allEvents
                .Where(e => e.CreatedDate >= effectiveWindowStart && e.CreatedDate < windowEnd)
                .ToList();

            var byAddress = new List<DuplicateDetailV2>();

            foreach (var address in allAddresses)
            {
                var addressEvents = windowEvents
                    .Where(e => e.EventAddressV2Id == address.Id)
                    .ToList();

                if (addressEvents.Count == 0)
                {
                    continue;
                }

                var participantGroups = addressEvents
                    .GroupBy(e => e.EventParticipantV2Id ?? Guid.Empty)
                    .ToList();

                foreach (var participantGroup in participantGroups)
                {
                    var participantEvents = participantGroup.ToList();

                    var contentHashGroups = participantEvents
                        .GroupBy(e => e.ContentHash)
                        .ToList();

                    int totalEvents = participantEvents.Count;
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

                    var participant = participantEvents[0].EventParticipantV2;

                    bool isKnownParticipant =
                        participantGroup.Key != Guid.Empty && participant != null;

                    byAddress.Add(new DuplicateDetailV2
                    {
                        EventAddressV2Id = address.Id,
                        EventAddressV2Name = address.Name,
                        EventParticipantV2Id = isKnownParticipant ? participantGroup.Key : (Guid?)null,
                        EventParticipantV2Name = isKnownParticipant ? participant.Name : "Unknown",
                        TotalEvents = totalEvents,
                        Duplicates = duplicates,
                        DuplicateRate = duplicateRate,
                        LastDuplicateSeen = lastDuplicateSeen
                    });
                }
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
