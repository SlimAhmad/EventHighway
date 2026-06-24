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
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
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
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOnReplay(startDate, endDate);

            BatchConfiguration batchConfiguration =
                this.configurationBroker.GetBatchConfiguration();

            int take = batchConfiguration.BatchSizeForBulkProcessing;
            int skip = 0;
            IEnumerable<ListenerEventArchiveV2> listenerArchiveBatch;

            do
            {
                listenerArchiveBatch = await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId, eventListenerIds, startDate, endDate, skip, take, cancellationToken);

                if (!listenerArchiveBatch.Any())
                    break;

                IEnumerable<Guid> eventArchiveIds =
                    listenerArchiveBatch
                        .Select(listenerEventArchiveV2 => listenerEventArchiveV2.EventArchiveV2Id)
                        .Distinct()
                        .ToList();

                IEnumerable<EventArchiveV2> eventArchiveV2s =
                    await this.eventArchiveV2OrchestrationService
                        .RetrieveEventArchiveV2sByIdsAsync(eventArchiveIds, cancellationToken);

                await this.restoringEventV2OrchestrationService
                    .RestoreAsync(eventArchiveV2s, listenerArchiveBatch, cancellationToken);

                if (eventListenerIds is null || !eventListenerIds.Any())
                {
                    await this.restoringEventV2OrchestrationService
                        .GenerateReplayForNewListenersAsync(eventArchiveV2s, cancellationToken);
                }

                if (take == 0)
                    break;

                skip += take;
            }
            while (true);
        });
    }
}
