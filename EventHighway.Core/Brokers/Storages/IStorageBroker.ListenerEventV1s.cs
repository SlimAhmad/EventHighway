// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<ListenerEventV1> InsertListenerEventV1Async(
            ListenerEventV1 listenerEventV1,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventV1>> SelectAllListenerEventV1sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV1> SelectListenerEventV1ByIdAsync(
            Guid listenerEventV1Id,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV1> UpdateListenerEventV1Async(
            ListenerEventV1 listenerEventV1,
            CancellationToken cancellationToken = default);

        ValueTask<ListenerEventV1> DeleteListenerEventV1Async(
            ListenerEventV1 listenerEventV1,
            CancellationToken cancellationToken = default);
    }
}
