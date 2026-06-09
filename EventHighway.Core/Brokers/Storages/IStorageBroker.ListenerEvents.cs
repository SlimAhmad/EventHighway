// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<ListenerEvent> InsertListenerEventAsync(ListenerEvent listenerEvent, CancellationToken cancellationToken = default);
        ValueTask<ListenerEvent> UpdateListenerEventAsync(ListenerEvent listenerEvent, CancellationToken cancellationToken = default);
    }
}
