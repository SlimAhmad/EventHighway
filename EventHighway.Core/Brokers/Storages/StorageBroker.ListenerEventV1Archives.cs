// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<ListenerEventArchiveV1> ListenerEventV1Archives { get; set; }

        public async ValueTask<ListenerEventArchiveV1> InsertListenerEventV1ArchiveAsync(
            ListenerEventArchiveV1 listenerEventV1Archive)
        {
            return await InsertAsync(listenerEventV1Archive);
        }
    }
}
