// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
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
        private readonly ILoggingBroker loggingBroker;

        public HealthV2CoordinationService(
            IEventV2OrchestrationService eventV2OrchestrationService,
            IEventListenerV2OrchestrationService eventListenerV2OrchestrationService,
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            ILoggingBroker loggingBroker)
        {
            this.eventV2OrchestrationService = eventV2OrchestrationService;
            this.eventListenerV2OrchestrationService = eventListenerV2OrchestrationService;
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthSummaryV2Async(
            CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();
    }
}
