// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventV1> EventV1s { get; set; }

        public async ValueTask<EventV1> InsertEventV1Async(
            EventV1 eventV1,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventV1, cancellationToken);

        public async ValueTask<IQueryable<EventV1>> SelectAllEventV1sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventV1>(cancellationToken);

        public async ValueTask<IQueryable<EventV1>> SelectAllEventV1sWithListenerEventV1sAsync(
            CancellationToken cancellationToken = default) =>
            (await SelectAllAsync<EventV1>(cancellationToken))
                .Include(eventV1 => eventV1.ListenerEvents);

        public async ValueTask<EventV1> SelectEventV1ByIdAsync(
            Guid eventV1Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventV1>(new object[] { eventV1Id }, cancellationToken);

        public async ValueTask<EventV1> UpdateEventV1Async(
            EventV1 eventV1,
            CancellationToken cancellationToken = default) =>
            await UpdateAsync(eventV1, cancellationToken);

        public async ValueTask<EventV1> DeleteEventV1Async(
            EventV1 eventV1,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventV1, cancellationToken);
    }
}
