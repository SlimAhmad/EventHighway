// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventListenerV1> EventListenerV1s { get; set; }

        public async ValueTask<EventListenerV1> InsertEventListenerV1Async(
            EventListenerV1 eventListenerV1,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventListenerV1, cancellationToken);

        public async ValueTask<IQueryable<EventListenerV1>> SelectAllEventListenerV1sAsync() =>
            SelectAll<EventListenerV1>();

        public async ValueTask<EventListenerV1> SelectEventListenerV1ByIdAsync(
            Guid eventListenerV1Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventListenerV1>(new object[] { eventListenerV1Id }, cancellationToken);

        public async ValueTask<EventListenerV1> DeleteEventListenerV1Async(
            EventListenerV1 eventListenerV1,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventListenerV1, cancellationToken);
    }
}
