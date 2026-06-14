// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventArchiveV1> EventArchiveV1s { get; set; }

        public async ValueTask<EventArchiveV1> InsertEventArchiveV1Async(
            EventArchiveV1 eventArchiveV1,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventArchiveV1, cancellationToken);

        public async ValueTask<IQueryable<EventArchiveV1>> SelectAllEventArchiveV1sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventArchiveV1>(cancellationToken);

        public async ValueTask<EventArchiveV1> SelectEventArchiveV1ByIdAsync(
            Guid eventArchiveV1Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventArchiveV1>(new object[] { eventArchiveV1Id }, cancellationToken);

        public async ValueTask<EventArchiveV1> DeleteEventArchiveV1Async(
            EventArchiveV1 eventArchiveV1V1,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventArchiveV1V1, cancellationToken);
    }
}
