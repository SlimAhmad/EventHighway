// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
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
        TryCatch(async () =>
        {
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

            double errorRate = totalListenerEvents > 0
                ? (double)errorListenerEvents / totalListenerEvents * 100
                : 0;

            int totalArchivedEvents = allArchivedEvents.Count();
            int totalArchivedListenerEvents = allArchivedListenerEvents.Count();

            int archivedListenerErrors =
                allArchivedListenerEvents.Count(la =>
                    la.Status == ListenerEventArchiveStatusV2.Error);

            int handlerCount = allHandlers.Count();

            EventHighwayConfiguration config =
                this.configurationBroker.GetEventHighwayConfiguration();

            HealthStatusV2 deadEventsStatus =
                deadEvents == 0 ? HealthStatusV2.Green
                : deadEvents <= 5 ? HealthStatusV2.Amber
                : HealthStatusV2.Red;

            HealthStatusV2 errorRateStatus =
                errorRate < 10 ? HealthStatusV2.Green
                : errorRate <= 25 ? HealthStatusV2.Amber
                : HealthStatusV2.Red;

            HealthStatusV2 handlerStatus =
                ComputeRagStatus(handlerCount, HealthMetric.HandlerCount, config);

            return new List<HealthCheckItemV2>
            {
                CreateItem("Event Addresses", "Total", totalAddresses.ToString(), null, HealthStatusV2.NA),
                CreateItem("Event Listeners", "Total", totalListeners.ToString(), null, HealthStatusV2.NA),
                CreateItem("Active Events", "Total", totalEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Active Events", "Immediate", immediateEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Active Events", "Scheduled", scheduledEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Active Events", "Dead (0 retries)", deadEvents.ToString(), null, deadEventsStatus),
                CreateItem("Listener Events", "Total", totalListenerEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Listener Events", "Pending", pendingListenerEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Listener Events", "Successful", successListenerEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Listener Events", "Errors", errorListenerEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Listener Events", "Error Rate %", $"{errorRate:F2}", null, errorRateStatus),
                CreateItem("Event Archives", "Total Archived Events", totalArchivedEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Event Archives", "Total Archived Listener Events", totalArchivedListenerEvents.ToString(), null, HealthStatusV2.NA),
                CreateItem("Event Archives", "Archived Listener Errors", archivedListenerErrors.ToString(), null, HealthStatusV2.NA),
                CreateItem("Event Handlers", "Registered Handlers", handlerCount.ToString(), null, handlerStatus),
            };
        });

        private static HealthStatusV2 ComputeRagStatus(
            decimal value,
            HealthMetric metric,
            EventHighwayConfiguration config)
        {
            RagThreshold threshold =
                config.Health.Thresholds.FirstOrDefault(t => t.Metric == metric);

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
            string description,
            HealthStatusV2 status)
        {
            return new HealthCheckItemV2
            {
                Grouping = grouping,
                Item = item,
                Value = value,
                Description = description,
                StatusCode = (int)status,
                Status = status.ToString()
            };
        }
    }
}
