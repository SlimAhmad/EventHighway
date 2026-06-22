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
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Processings.Events.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    internal partial class ArchivingEventV2OrchestrationService : IArchivingEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IListenerEventV2ProcessingService listenerEventV2ProcessingService;
        private readonly IConfigurationBroker configurationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ArchivingEventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IListenerEventV2ProcessingService listenerEventV2ProcessingService,
            IConfigurationBroker configurationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.listenerEventV2ProcessingService = listenerEventV2ProcessingService;
            this.configurationBroker = configurationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IEnumerable<EventV2>> RetrieveBatchOfQuarantinedEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            ValidateOnRetrieveBatchOfQuarantined(batchConfiguration);

            LoopDetection loopDetection =
                this.configurationBroker.GetLoopDetectionConfiguration();

            DateTimeOffset now =
                await this.dateTimeBroker.GetDateTimeOffsetAsync();

            DateTimeOffset cutoff = now - loopDetection.Window;

            IQueryable<EventV2> allEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllEventV2sAsync(cancellationToken);

            IQueryable<EventV2> quarantinedEventV2s =
                allEventV2s.Where(eventV2 =>
                    eventV2.Status == EventStatusV2.Quarantined
                    && eventV2.CreatedDate < cutoff);

            int take = batchConfiguration.BatchSizeForBulkProcessing;

            return take == 0
                ? quarantinedEventV2s.AsEnumerable()
                : quarantinedEventV2s.Take(take).AsEnumerable();
        });

        public ValueTask<IEnumerable<EventV2>> RetrieveBatchOfDeadEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            ValidateOnRetrieveBatchOfDead(batchConfiguration);

            IQueryable<EventV2> deadEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sWithListenersAsync(cancellationToken);

            int take = batchConfiguration.BatchSizeForBulkProcessing;

            return take == 0
                ? deadEventV2s.AsEnumerable()
                : deadEventV2s.Take(take).AsEnumerable();
        });

        public ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfListenerEventV2sAsync(
            IEnumerable<Guid> eventV2Ids,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            ValidateOnRetrieveBatchOfListenerEventV2s(eventV2Ids, batchConfiguration);

            int take = batchConfiguration.BatchSizeForBulkProcessing;

            return await this.listenerEventV2ProcessingService
                .RetrieveBatchOfListenerEventV2sByEventIdsAsync(eventV2Ids, take, cancellationToken);
        });

        public ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateListenerEventV2sIsNotNull(listenerEventV2s);

            await this.listenerEventV2ProcessingService
                .BulkRemoveListenerEventV2sAsync(listenerEventV2s, cancellationToken);
        });

        public ValueTask BulkRemoveEventV2sAsync(
            IEnumerable<EventV2> eventV2s,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventV2sIsNotNull(eventV2s);

            await this.eventV2ProcessingService
                .BulkRemoveEventV2sAsync(eventV2s, cancellationToken);
        });

        public ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
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
