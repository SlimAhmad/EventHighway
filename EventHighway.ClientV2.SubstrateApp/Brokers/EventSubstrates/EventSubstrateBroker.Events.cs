// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public ValueTask<EventV2> SubmitEventAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventV2Client
                .SubmitEventV2Async(eventV2, cancellationToken);

        public ValueTask FirePendingEventsAsync(CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventV2Client
                .FireScheduledPendingEventV2sAsync(cancellationToken);
    }
}
