// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventArchiveV1> EventV1Archives { get; set; }

        public async ValueTask<EventArchiveV1> InsertEventV1ArchiveAsync(EventArchiveV1 eventV1Archive) =>
            await InsertAsync(eventV1Archive);

        public async ValueTask<IQueryable<EventArchiveV1>> SelectAllEventArchiveV1sAsync() =>
            SelectAll<EventArchiveV1>();

        public async ValueTask<EventArchiveV1> SelectEventArchiveV1ByIdAsync(Guid eventV1ArchiveId) =>
            await SelectAsync<EventArchiveV1>(eventV1ArchiveId);

        public async ValueTask<EventArchiveV1> DeleteEventV1ArchiveAsync(EventArchiveV1 eventV1ArchiveV1) =>
            await DeleteAsync(eventV1ArchiveV1);
    }
}
