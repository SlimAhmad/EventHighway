// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventListener> InsertEventListenerAsync(EventListener eventListener, CancellationToken cancellationToken = default);
        ValueTask<IQueryable<EventListener>> SelectAllEventListenersAsync();
    }
}
