// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventListener> EventListeners { get; set; }

        public async ValueTask<EventListener> InsertEventListenerAsync(
            EventListener eventListener,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventListener, cancellationToken);

        public async ValueTask<IQueryable<EventListener>> SelectAllEventListenersAsync() =>
            SelectAll<EventListener>();
    }
}
