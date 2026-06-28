// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask ReplayEventToListenersAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default);

        ValueTask ProcessReplayedEventsAsync(CancellationToken cancellationToken = default);
    }
}
