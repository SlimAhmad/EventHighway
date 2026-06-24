// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
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

        public async ValueTask RestoreAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default)
        {
            List<EventV2> mappedEventV2s =
                eventArchiveV2s.Select(MapToEventV2).ToList();

            IQueryable<EventV2> existingEventV2s =
                await this.eventV2ProcessingService.RetrieveAllEventV2sAsync(cancellationToken);

            HashSet<System.Guid> existingEventV2Ids =
                existingEventV2s.Select(eventV2 => eventV2.Id).ToHashSet();

            List<EventV2> eventV2sToRestore =
                mappedEventV2s
                    .Where(eventV2 => existingEventV2Ids.Contains(eventV2.Id) is false)
                        .ToList();

            await this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                eventV2sToRestore, cancellationToken);

            List<ListenerEventV2> mappedListenerEventV2s =
                listenerEventArchiveV2s.Select(MapToListenerEventV2).ToList();

            IQueryable<ListenerEventV2> existingListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            HashSet<System.Guid> existingListenerEventV2Ids =
                existingListenerEventV2s.Select(listenerEventV2 => listenerEventV2.Id).ToHashSet();

            List<ListenerEventV2> listenerEventV2sToRestore =
                mappedListenerEventV2s
                    .Where(listenerEventV2 =>
                        existingListenerEventV2Ids.Contains(listenerEventV2.Id) is false)
                            .ToList();

            await this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                listenerEventV2sToRestore, cancellationToken);
        }

        public async ValueTask GenerateReplayForNewListenersAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            CancellationToken cancellationToken = default)
        {
            IQueryable<ListenerEventV2> existingListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);

            HashSet<(System.Guid EventId, System.Guid EventListenerId)> existingPairs =
                existingListenerEventV2s
                    .ToList()
                    .Select(listenerEventV2 =>
                        (listenerEventV2.EventId, listenerEventV2.EventListenerId))
                            .ToHashSet();

            List<ListenerEventV2> generatedListenerEventV2s = new List<ListenerEventV2>();

            foreach (EventArchiveV2 eventArchiveV2 in eventArchiveV2s)
            {
                IQueryable<EventListenerV2> eventListenerV2s =
                    await this.eventListenerV2ProcessingService
                        .RetrieveEventListenerV2sByEventAddressIdAsync(
                            eventArchiveV2.EventAddressId, cancellationToken);

                foreach (EventListenerV2 eventListenerV2 in eventListenerV2s)
                {
                    bool hasListenerEvent =
                        existingPairs.Contains((eventArchiveV2.Id, eventListenerV2.Id));

                    if (hasListenerEvent is false)
                    {
                        generatedListenerEventV2s.Add(
                            GenerateListenerEventV2(eventArchiveV2, eventListenerV2));
                    }
                }
            }

            await this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                generatedListenerEventV2s, cancellationToken);
        }

        private static ListenerEventV2 GenerateListenerEventV2(
            EventArchiveV2 eventArchiveV2,
            EventListenerV2 eventListenerV2) =>
            new ListenerEventV2
            {
                Id = System.Guid.NewGuid(),
                Status = ListenerEventStatusV2.Replay,
                Response = null,
                ResponseCode = null,
                ResponseMessage = null,
                CreatedDate = eventArchiveV2.CreatedDate,
                UpdatedDate = eventArchiveV2.CreatedDate,
                EventId = eventArchiveV2.Id,
                EventAddressId = eventArchiveV2.EventAddressId,
                EventListenerId = eventListenerV2.Id
            };

        private static EventV2 MapToEventV2(EventArchiveV2 eventArchiveV2) =>
            new EventV2
            {
                Id = eventArchiveV2.Id,
                Content = eventArchiveV2.Content,
                EventName = eventArchiveV2.EventName,
                Type = EventTypeV2.Immediate,
                Status = EventStatusV2.Active,
                CreatedDate = eventArchiveV2.CreatedDate,
                UpdatedDate = eventArchiveV2.UpdatedDate,
                ScheduledDate = null,
                ContentHash = null,
                RemainingRetryAttempts = 0,
                EventAddressId = eventArchiveV2.EventAddressId
            };

        private static ListenerEventV2 MapToListenerEventV2(
            ListenerEventArchiveV2 listenerEventArchiveV2) =>
            new ListenerEventV2
            {
                Id = listenerEventArchiveV2.Id,
                Status = ListenerEventStatusV2.Replay,
                Response = null,
                ResponseCode = null,
                ResponseMessage = null,
                CreatedDate = listenerEventArchiveV2.CreatedDate,
                UpdatedDate = listenerEventArchiveV2.UpdatedDate,
                EventId = listenerEventArchiveV2.EventId,
                EventAddressId = listenerEventArchiveV2.EventAddressId,
                EventListenerId = listenerEventArchiveV2.EventListenerId
            };
    }
}
