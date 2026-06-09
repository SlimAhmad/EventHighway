// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventV1> ListenerEventV1s { get; set; }

        public async ValueTask<ListenerEventV1> InsertListenerEventV1Async(
            ListenerEventV1 listenerEventV1,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(listenerEventV1, cancellationToken);

        public async ValueTask<IQueryable<ListenerEventV1>> SelectAllListenerEventV1sAsync() =>
            SelectAll<ListenerEventV1>();

        public async ValueTask<ListenerEventV1> SelectListenerEventV1ByIdAsync(
            Guid listenerEventV1Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<ListenerEventV1>(new object[] { listenerEventV1Id }, cancellationToken);

        public async ValueTask<ListenerEventV1> UpdateListenerEventV1Async(
            ListenerEventV1 listenerEventV1,
            CancellationToken cancellationToken = default) =>
            await UpdateAsync(listenerEventV1, cancellationToken);

        public async ValueTask<ListenerEventV1> DeleteListenerEventV1Async(
            ListenerEventV1 listenerEventV1,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(listenerEventV1, cancellationToken);
    }
}
