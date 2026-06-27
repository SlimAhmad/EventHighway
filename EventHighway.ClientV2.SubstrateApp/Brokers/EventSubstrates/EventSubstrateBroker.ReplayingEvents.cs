// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public ValueTask ReplayEventToListenersAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ReplayingEventV2Client
                .ReplayEventArchiveV2sAsync(
                    eventV2Id,
                    eventAddressId,
                    eventListenerIds,
                    allowReplayOfQuarantinedItem,
                    cancellationToken);

        public ValueTask ProcessReplayedEventsAsync(CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ReplayingEventV2Client
                .ProcessReplayedListenerEventV2sAsync(cancellationToken);
    }
}
