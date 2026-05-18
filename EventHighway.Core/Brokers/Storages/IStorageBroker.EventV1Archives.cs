// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventArchiveV1> InsertEventV1ArchiveAsync(EventArchiveV1 eventV1Archive);
        ValueTask<IQueryable<EventArchiveV1>> SelectAllEventArchiveV1sAsync();
        ValueTask<EventArchiveV1> SelectEventArchiveV1ByIdAsync(Guid eventV1ArchiveId);
        ValueTask<EventArchiveV1> DeleteEventV1ArchiveAsync(EventArchiveV1 eventV1ArchiveV1);
    }
}
