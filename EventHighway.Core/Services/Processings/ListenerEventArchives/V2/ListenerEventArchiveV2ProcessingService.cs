// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
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

        public ValueTask BulkRemoveListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s, 
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.listenerEventArchiveV2Service.BulkRemoveListenerEventArchiveV2sAsync(
                listenerEventArchiveV2s, cancellationToken);
        });
    }
}
