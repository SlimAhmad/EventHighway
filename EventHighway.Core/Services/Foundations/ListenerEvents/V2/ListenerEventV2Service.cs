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
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Foundations.ListenerEvents.V2
{
    internal partial class ListenerEventV2Service : IListenerEventV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<ListenerEventV2> AddListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await ValidateListenerEventV2OnAddAsync(listenerEventV2);

            return await storageBroker.InsertListenerEventV2Async(listenerEventV2, cancellationToken);
        });

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync() =>
        TryCatch(async () => await this.storageBroker.SelectAllListenerEventV2sAsync());

        public ValueTask<ListenerEventV2> ModifyListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await ValidateListenerEventV2OnModifyAsync(listenerEventV2);

            ListenerEventV2 maybeListenerEventV2 =
                await this.storageBroker.SelectListenerEventV2ByIdAsync(
                    listenerEventV2.Id,
                    cancellationToken);

            ValidateListenerEventV2AgainstStorage(listenerEventV2, maybeListenerEventV2);

            return await storageBroker.UpdateListenerEventV2Async(listenerEventV2, cancellationToken);
        });

        public ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateListenerEventV2Id(listenerEventV2Id);

            ListenerEventV2 maybeListenerEventV2 =
                await this.storageBroker.SelectListenerEventV2ByIdAsync(
                    listenerEventV2Id,
                    cancellationToken);

            ValidateListenerEventV2Exists(maybeListenerEventV2, listenerEventV2Id);

            return await this.storageBroker.DeleteListenerEventV2Async(maybeListenerEventV2, cancellationToken);
        });

        public async ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default)
        {
            await this.storageBroker.BulkDeleteListenerEventV2sAsync(listenerEventV2s, cancellationToken);
        }
    }
}
