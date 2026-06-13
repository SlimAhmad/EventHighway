// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEvent2OrchestrationService : IArchivingEvent2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEvent2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventV2>> RetrieveAllDeadEventV2sWithListenersAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventV2ProcessingService.RetrieveAllDeadEventV2sWithListenersAsync());

        public ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2IsNotNull(eventV2);

            foreach (ListenerEventV2 listenerEventV2 in eventV2.ListenerEventV2s)
            {
                await this.listenerEventV2ProcessingService
                    .RemoveListenerEventV2ByIdAsync(listenerEventV2.Id, cancellationToken);
            }

            await this.eventV2ProcessingService.RemoveEventV2ByIdAsync(eventV2.Id, cancellationToken);
        });
    }
}
