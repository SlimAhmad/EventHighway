// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2
{
    internal partial class ReplayingListenerEventV2OrchestrationService
        : IReplayingListenerEventV2OrchestrationService
    {
        private readonly IEventCallV2ProcessingService eventCallV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ReplayingListenerEventV2OrchestrationService(
            IEventCallV2ProcessingService eventCallV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventCallV2ProcessingService = eventCallV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV2> ProcessReplayListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();
    }
}
