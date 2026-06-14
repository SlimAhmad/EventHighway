// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2Service : IListenerEventArchiveV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventArchiveV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventArchiveV2> AddListenerEventArchiveV2Async(
            ListenerEventArchiveV2 listenerEventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await ValidateListenerEventArchiveV2OnAddAsync(listenerEventArchiveV2);

            return await this.storageBroker.InsertListenerEventArchiveV2Async(
                listenerEventArchiveV2,
                cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.storageBroker.SelectAllListenerEventArchiveV2sAsync();
        });

        public ValueTask BulkRemoveListenerEventArchiveV2sAsync(
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
