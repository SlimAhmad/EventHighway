// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEvent> ListenerEvents { get; set; }

        public async ValueTask<ListenerEvent> InsertListenerEventAsync(
            ListenerEvent listenerEvent,
            CancellationToken cancellationToken = default) =>
            await this.InsertAsync(listenerEvent, cancellationToken);

        public async ValueTask<ListenerEvent> UpdateListenerEventAsync(
            ListenerEvent listenerEvent,
            CancellationToken cancellationToken = default) =>
            await this.UpdateAsync(listenerEvent, cancellationToken);
    }
}
