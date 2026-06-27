// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventsAsync(
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ListenerEventV2Client
                .RetrieveAllListenerEventV2sAsync(cancellationToken);
    }
}
