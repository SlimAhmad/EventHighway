// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public ValueTask ArchiveEventsAsync(CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ArchivingEventV2Client
                .ArchiveEventV2sAsync(cancellationToken);
    }
}
