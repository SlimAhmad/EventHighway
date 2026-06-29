// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public async ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventAddressV2Client
                .RegisterEventAddressV2Async(eventAddressV2, cancellationToken);

        public async ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventAddressV2Client
                .RetrieveAllEventAddressV2sAsync(cancellationToken);

        public async ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default) =>
            await this.clientV2.EventAddressV2Client
                .RemoveEventAddressV2ByIdAsync(eventAddressV2Id, cancellationToken);
    }
}
