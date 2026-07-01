// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ReplayingEventV2Client.ReplayEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, cancellationToken),
                cancellationToken);

        public ValueTask ReplayEventArchiveV2sAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ReplayingEventV2Client.ReplayEventArchiveV2sAsync(
                    eventV2Id,
                    eventAddressId,
                    eventListenerIds,
                    allowReplayOfQuarantinedItem,
                    cancellationToken),
                cancellationToken);

        public ValueTask ProcessReplayedListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
            this.clientV2Provider.ExecuteAsync(client =>
                client.ReplayingEventV2Client.ProcessReplayedListenerEventV2sAsync(
                    cancellationToken),
                cancellationToken);
    }
}
