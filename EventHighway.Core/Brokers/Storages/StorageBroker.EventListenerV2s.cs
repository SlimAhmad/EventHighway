// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventListenerV2> EventListenerV2s { get; set; }

        public async ValueTask<EventListenerV2> InsertEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default)
        {
            var broker = new StorageBroker(this.connectionString);
            broker.Add(eventListenerV2);
            await broker.SaveChangesAsync(cancellationToken);

            return eventListenerV2;
        }

        public async ValueTask<IQueryable<EventListenerV2>> SelectAllEventListenerV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventListenerV2>(cancellationToken);

        public async ValueTask<EventListenerV2> SelectEventListenerV2ByIdAsync(
            Guid eventListenerV2Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventListenerV2>(new object[] { eventListenerV2Id }, cancellationToken);

        public async ValueTask<EventListenerV2> DeleteEventListenerV2Async(
            EventListenerV2 eventListenerV2,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventListenerV2, cancellationToken);
    }
}
