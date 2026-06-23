// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.RestoringEvents.V2
{
    internal partial class RestoringEventV2OrchestrationService : IRestoringEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IEventListenerV2ProcessingService eventListenerV2ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public RestoringEventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IEventListenerV2ProcessingService eventListenerV2ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.eventListenerV2ProcessingService = eventListenerV2ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask RestoreAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
