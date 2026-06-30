// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.ListenerEventArchives;

namespace EventHighway.Portal.Web.Services.Views.ListenerEventArchives
{
    public interface IListenerEventArchivesViewService
    {
        ValueTask<List<ListenerEventArchiveView>> RetrieveAllListenerEventArchivesAsync(
            CancellationToken cancellationToken = default);

        ValueTask<List<ListenerEventArchiveView>>
            RetrieveListenerEventArchivesByEventArchiveIdAsync(
                Guid eventArchiveId,
                CancellationToken cancellationToken = default);

        ValueTask<ListenerEventArchiveView> RetrieveListenerEventArchiveByIdAsync(
            Guid listenerEventArchiveId,
            CancellationToken cancellationToken = default);
    }
}
