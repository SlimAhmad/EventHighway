// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
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
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2CoordinationService(
            IArchivingEventV2OrchestrationService archivingEventV2OrchestrationService,
            IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.archivingEventV2OrchestrationService = archivingEventV2OrchestrationService;
            this.eventArchiveV2OrchestrationService = eventArchiveV2OrchestrationService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            BatchConfiguration batchConfiguration = this.configurationBroker.GetBatchConfiguration();
            int take = batchConfiguration.BatchSizeForBulkProcessing;
            IEnumerable<EventArchiveV2> batch;

            do
            {
                batch = await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(olderThan, take);

                if (!batch.Any())
                    break;

                await this.eventArchiveV2OrchestrationService
                    .BulkRemoveEventArchiveV2sAsync(batch, cancellationToken);
            }
            while (true);
        });

        public ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var faultedEventV2Ids = new HashSet<Guid>();
            var failedEventV2Ids = new List<Guid>();
            var failedListenerEventV2Ids = new List<Guid>();

            IEnumerable<EventV2> deadEventV2s;

            do
            {
                deadEventV2s = await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfDeadEventV2sAsync(cancellationToken);

                IEnumerable<EventV2> pendingDeadEventV2s =
                    deadEventV2s.Where(eventV2 =>
                        !faultedEventV2Ids.Contains(eventV2.Id)).ToList();

                if (!pendingDeadEventV2s.Any())
                    break;

                IEnumerable<EventArchiveV2> eventArchiveV2s =
                    pendingDeadEventV2s.Select(MapToEventArchiveV2).ToList();

                IEnumerable<EventArchiveV2> addedEventArchiveV2s =
                    await this.eventArchiveV2OrchestrationService
                        .BulkAddEventArchiveV2sAsync(eventArchiveV2s, cancellationToken);

                var archivedEventV2Ids =
                    addedEventArchiveV2s.Select(eventArchiveV2 => eventArchiveV2.Id).ToHashSet();

                foreach (EventV2 unarchivedEventV2 in pendingDeadEventV2s
                    .Where(eventV2 => !archivedEventV2Ids.Contains(eventV2.Id)))
                {
                    if (faultedEventV2Ids.Add(unarchivedEventV2.Id))
                        failedEventV2Ids.Add(unarchivedEventV2.Id);
                }

                IEnumerable<Guid> pendingEventV2Ids = archivedEventV2Ids.ToList();

                IEnumerable<ListenerEventV2> listenerEventV2s;

                do
                {
                    listenerEventV2s = await this.archivingEventV2OrchestrationService
                        .RetrieveBatchOfListenerEventV2sAsync(pendingEventV2Ids, cancellationToken);

                    if (!listenerEventV2s.Any())
                        break;

                    IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s =
                        listenerEventV2s.Select(MapToListenerEventArchiveV2).ToList();

                    IEnumerable<ListenerEventArchiveV2> addedListenerEventArchiveV2s =
                        await this.eventArchiveV2OrchestrationService
                            .BulkAddListenerEventArchiveV2sAsync(listenerEventArchiveV2s, cancellationToken);

                    var addedListenerEventArchiveIds =
                        addedListenerEventArchiveV2s.Select(listenerEventArchiveV2 =>
                            listenerEventArchiveV2.Id).ToHashSet();

                    IEnumerable<ListenerEventV2> addedListenerEventV2s =
                        listenerEventV2s
                            .Where(listenerEventV2 =>
                                addedListenerEventArchiveIds.Contains(listenerEventV2.Id)).ToList();

                    if (addedListenerEventV2s.Any())
                    {
                        await this.archivingEventV2OrchestrationService
                            .BulkRemoveListenerEventV2sAsync(addedListenerEventV2s, cancellationToken);
                    }

                    foreach (ListenerEventV2 unarchivedListenerEventV2 in listenerEventV2s
                        .Where(listenerEventV2 =>
                            !addedListenerEventArchiveIds.Contains(listenerEventV2.Id)))
                    {
                        failedListenerEventV2Ids.Add(unarchivedListenerEventV2.Id);
                        faultedEventV2Ids.Add(unarchivedListenerEventV2.EventId);
                    }

                    pendingEventV2Ids =
                        pendingEventV2Ids.Where(eventV2Id =>
                            !faultedEventV2Ids.Contains(eventV2Id)).ToList();

                    if (!pendingEventV2Ids.Any())
                        break;
                }
                while (true);

                IEnumerable<EventV2> removableEventV2s =
                    pendingDeadEventV2s.Where(eventV2 =>
                        archivedEventV2Ids.Contains(eventV2.Id)
                            && !faultedEventV2Ids.Contains(eventV2.Id)).ToList();

                if (removableEventV2s.Any())
                {
                    await this.archivingEventV2OrchestrationService
                        .BulkRemoveEventV2sAsync(removableEventV2s, cancellationToken);
                }
            }
            while (true);

            if (failedEventV2Ids.Any() || failedListenerEventV2Ids.Any())
            {
                await LogFailedArchivingEventV2sAsync(failedEventV2Ids, failedListenerEventV2Ids);
            }
        });

        private async ValueTask LogFailedArchivingEventV2sAsync(
            IEnumerable<Guid> failedEventV2Ids,
            IEnumerable<Guid> failedListenerEventV2Ids)
        {
            var failedArchivingEventV2CoordinationException =
                new FailedArchivingEventV2CoordinationException(
                    message: "Some dead events could not be fully archived " +
                        "and were retained for the next run.");

            if (failedEventV2Ids.Any())
            {
                failedArchivingEventV2CoordinationException.AddData(
                    key: "failedEventV2Ids",
                    values: failedEventV2Ids.Select(id => id.ToString()).ToArray());
            }

            if (failedListenerEventV2Ids.Any())
            {
                failedArchivingEventV2CoordinationException.AddData(
                    key: "failedListenerEventV2Ids",
                    values: failedListenerEventV2Ids.Select(id => id.ToString()).ToArray());
            }

            await this.loggingBroker.LogErrorAsync(failedArchivingEventV2CoordinationException);
        }

        private static EventArchiveV2 MapToEventArchiveV2(EventV2 eventV2)
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
                EventAddressId = eventV2.EventAddressId
            };
        }

        private static ListenerEventArchiveV2 MapToListenerEventArchiveV2(
            ListenerEventV2 listenerEventV2)
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
                EventId = listenerEventV2.EventId,
                EventAddressId = listenerEventV2.EventAddressId,
                EventListenerId = listenerEventV2.EventListenerId,
                EventArchiveV2Id = listenerEventV2.EventId
            };
        }
    }
}
