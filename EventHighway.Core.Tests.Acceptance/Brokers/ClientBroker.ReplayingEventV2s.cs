// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate) =>
            await this.eventHighwayClient.V2.ReplayingEventV2Client
                .ReplayEventArchiveV2sAsync(
                    eventAddressId,
                    eventListenerIds,
                    startDate,
                    endDate);

        public async ValueTask ProcessReplayedListenerEventV2sAsync() =>
            await this.eventHighwayClient.V2.ReplayingEventV2Client
                .ProcessReplayedListenerEventV2sAsync();
    }
}
