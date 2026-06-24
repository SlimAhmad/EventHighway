// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.RestoringEvents.V2;

namespace EventHighway.Core.Services.Coordinations.ReplayingEvents.V2
{
    internal partial class ReplayingEventV2CoordinationService : IReplayingEventV2CoordinationService
    {
        private readonly IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService;
        private readonly IRestoringEventV2OrchestrationService restoringEventV2OrchestrationService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReplayingEventV2CoordinationService(
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            IRestoringEventV2OrchestrationService restoringEventV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.restoringEventV2OrchestrationService = restoringEventV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
