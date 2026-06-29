// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Portal.Web.Services.Views.EventArchives
{
    public interface IEventArchivesViewService
    {
        ValueTask ArchiveProcessedEventsAsync(
            CancellationToken cancellationToken = default);

        ValueTask PurgeArchivesOlderThanAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default);
    }
}
