// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V1;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2CoordinationService : IArchivingEventV2CoordinationService
    {
        private readonly IArchivingEvent2OrchestrationService archivingEvent2OrchestrationService;
        private readonly IEventArchiveV1OrchestrationService eventArchiveV1OrchestrationService;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2CoordinationService(
            IArchivingEvent2OrchestrationService archivingEvent2OrchestrationService,
            IEventArchiveV1OrchestrationService eventArchiveV1OrchestrationService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.archivingEvent2OrchestrationService = archivingEvent2OrchestrationService;
            this.eventArchiveV1OrchestrationService = eventArchiveV1OrchestrationService;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await foreach (EventV2 eventV2 in
                this.archivingEvent2OrchestrationService
                    .RetrieveAllDeadEventV2sWithListenersAsync(cancellationToken))
            {
                EventArchiveV1 eventArchiveV1 =
                    await MapToEventArchiveV1Async(eventV2);

                await this.eventArchiveV1OrchestrationService
                    .AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                        eventArchiveV1);

                await this.archivingEvent2OrchestrationService
                    .RemoveEventV2AndListenerEventV2sAsync(
                        eventV2,
                        cancellationToken);
            }
        });

        private async ValueTask<EventArchiveV1> MapToEventArchiveV1Async(EventV2 eventV2)
        {
            DateTimeOffset currentDateTime =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            return new EventArchiveV1
            {
                Id = eventV2.Id,
                Content = eventV2.Content,
                Type = (EventArchiveTypeV1)eventV2.Type,
                CreatedDate = eventV2.CreatedDate,
                UpdatedDate = eventV2.UpdatedDate,
                ScheduledDate = eventV2.ScheduledDate,
                ArchivedDate = currentDateTime,
                EventAddressId = eventV2.EventAddressId,

                ListenerEventArchiveV1s = eventV2.ListenerEventV2s
                    ?.Select(listenerEventV2 =>
                        MapToListenerEventArchiveV1(
                            listenerEventV2,
                            currentDateTime))
                                .ToList()
            };
        }

        private static ListenerEventArchiveV1 MapToListenerEventArchiveV1(
            ListenerEventV2 listenerEventV2,
            DateTimeOffset currentDateTime)
        {
            return new ListenerEventArchiveV1
            {
                Id = listenerEventV2.Id,
                Status = (ListenerEventArchiveStatusV1)listenerEventV2.Status,
                Response = listenerEventV2.Response,
                ResponseReasonPhrase = listenerEventV2.ResponseMessage,
                CreatedDate = listenerEventV2.CreatedDate,
                UpdatedDate = listenerEventV2.UpdatedDate,
                ArchivedDate = currentDateTime,
                EventId = listenerEventV2.EventId,
                EventAddressId = listenerEventV2.EventAddressId,
                EventListenerId = listenerEventV2.EventListenerId
            };
        }
    }
}
