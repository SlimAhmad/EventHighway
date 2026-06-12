// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Clients.ListenerEvents.V2
{
    public interface IListenerEventV2sClient
    {
        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default);
    }
}
