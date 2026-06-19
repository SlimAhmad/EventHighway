// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2CoordinationService : IArchivingEventV2CoordinationService
    {
        private readonly IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService;
        private readonly IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2CoordinationService(
            IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService,
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.archivingEventV2OrchestrationService = archivingEventV2OrchestrationService;
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            IEnumerable<EventV2> deadEventV2s;

            do
            {
                deadEventV2s = await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfDeadEventV2sAsync();

                if (!deadEventV2s.Any())
                    break;

                IEnumerable<EventArchiveV2> eventArchiveV2s =
                    deadEventV2s.Select(eventV2 =>
                        MapToEventArchiveV2(eventV2, currentDateTime)).ToList();

                IEnumerable<EventArchiveV2> addedEventArchiveV2s =
                    await this.eventArchiveV2OrchestrationService
                        .BulkAddEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);

                IEnumerable<Guid> archivedEventV2Ids =
                    addedEventArchiveV2s.Select(a => a.Id).ToList();

                IEnumerable<EventV2> archivedDeadEventV2s =
                    deadEventV2s.Where(e => archivedEventV2Ids.Contains(e.Id)).ToList();

                IEnumerable<ListenerEventV2> listenerEventV2s;

                do
                {
                    listenerEventV2s = await this.archivingEventV2OrchestrationService
                        .RetrieveBatchOfListenerEventV2sAsync(archivedEventV2Ids, cancellationToken);

                    if (!listenerEventV2s.Any())
                        break;

                    IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s =
                        listenerEventV2s.Select(listenerEventV2 =>
                            MapToListenerEventArchiveV2(listenerEventV2, currentDateTime)).ToList();

                    IEnumerable<ListenerEventArchiveV2> addedListenerEventArchiveV2s =
                        await this.eventArchiveV2OrchestrationService
                            .BulkAddListenerEventArchiveV2sAsync(listenerEventArchiveV2s, cancellationToken);

                    IEnumerable<Guid> addedListenerEventArchiveIds =
                        addedListenerEventArchiveV2s.Select(a => a.Id).ToList();

                    IEnumerable<ListenerEventV2> addedListenerEventV2s =
                        listenerEventV2s
                            .Where(l => addedListenerEventArchiveIds.Contains(l.Id)).ToList();

                    await this.archivingEventV2OrchestrationService
                        .BulkRemoveListenerEventV2sAsync(addedListenerEventV2s, cancellationToken);
                }
                while (true);

                await this.archivingEventV2OrchestrationService
                    .BulkRemoveEventV2sAsync(archivedDeadEventV2s, cancellationToken);
            }
            while (true);
        });

        private static EventArchiveV2 MapToEventArchiveV2(
            EventV2 eventV2,
            DateTimeOffset currentDateTime)
        {
            return new EventArchiveV2
            {
                Id = eventV2.Id,
                Content = eventV2.Content,
                EventName = eventV2.EventName,
                Type = (EventArchiveTypeV2)eventV2.Type,
                CreatedDate = eventV2.CreatedDate,
                UpdatedDate = eventV2.UpdatedDate,
                ScheduledDate = eventV2.ScheduledDate,
                RemainingRetryAttempts = eventV2.RemainingRetryAttempts,
                ArchivedDate = currentDateTime,
                EventAddressId = eventV2.EventAddressId
            };
        }

        private static ListenerEventArchiveV2 MapToListenerEventArchiveV2(
            ListenerEventV2 listenerEventV2,
            DateTimeOffset currentDateTime)
        {
            return new ListenerEventArchiveV2
            {
                Id = listenerEventV2.Id,
                Status = (ListenerEventArchiveStatusV2)listenerEventV2.Status,
                Response = listenerEventV2.Response,
                ResponseCode = listenerEventV2.ResponseCode,
                ResponseMessage = listenerEventV2.ResponseMessage,
                CreatedDate = listenerEventV2.CreatedDate,
                UpdatedDate = listenerEventV2.UpdatedDate,
                ArchivedDate = currentDateTime,
                EventId = listenerEventV2.EventId,
                EventAddressId = listenerEventV2.EventAddressId,
                EventListenerId = listenerEventV2.EventListenerId,
                EventArchiveV2Id = listenerEventV2.EventId
            };
        }
    }
}
