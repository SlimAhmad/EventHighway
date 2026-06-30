// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.Events;

namespace EventHighway.Portal.Web.Services.Views.Events
{
    public interface IEventsViewService
    {
        ValueTask<int> RetrieveArchivableEventCountAsync(
            CancellationToken cancellationToken = default);

        ValueTask<List<EventView>> RetrieveAllEventsAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventView> RetrieveEventByIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);
    }
}
