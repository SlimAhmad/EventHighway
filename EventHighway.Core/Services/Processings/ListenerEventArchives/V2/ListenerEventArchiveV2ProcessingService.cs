// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2ProcessingService : IListenerEventArchiveV2ProcessingService
    {
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventArchiveV2ProcessingService(
            IListenerEventArchiveV2Service listenerEventArchiveV2Service,
            ILoggingBroker loggingBroker)
        {
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync() =>
            TryCatch(async () =>
                await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync());

        public ValueTask<ListenerEventArchiveV2> AddListenerEventArchiveV2Async(
            ListenerEventArchiveV2 listenerEventArchiveV2,
            CancellationToken cancellationToken = default) => TryCatch(async () =>
        {
            ValidateListenerEventArchiveV2(listenerEventArchiveV2);

            return await this.listenerEventArchiveV2Service
                .AddListenerEventArchiveV2Async(listenerEventArchiveV2, cancellationToken);
        });

        public async ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveNextBatchOfArchivedEventV2sAsync(
            DateTimeOffset olderThan,
            int batchSize,
            CancellationToken cancellationToken)
        {
            IQueryable<ListenerEventArchiveV2> listenerEventArchiveV2 =
                await this.listenerEventArchiveV2Service.RetrieveAllListenerEventArchiveV2sAsync();

            listenerEventArchiveV2 = FilterListenerEventArchiveV2sOlderThan(
                olderThan, listenerEventArchiveV2)
                    .Take(batchSize);

            return listenerEventArchiveV2;
        }

        private static IQueryable<ListenerEventArchiveV2> FilterListenerEventArchiveV2sOlderThan(
            DateTimeOffset olderThan,
            IQueryable<ListenerEventArchiveV2> listenerEventArchiveV2s)
        {
            listenerEventArchiveV2s = listenerEventArchiveV2s.Where(
                listenerEventArchiveV2 => listenerEventArchiveV2.ArchivedDate < olderThan);

            return listenerEventArchiveV2s;
        }
    }
}
