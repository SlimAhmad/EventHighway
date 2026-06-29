// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public async ValueTask<IQueryable<EventListenerV2>>
            RetrieveEventListenerV2sByEventAddressIdAsync(
                Guid eventAddressV2Id,
                CancellationToken cancellationToken = default) =>
            await this.clientV2.EventListenerV2Client
                .RetrieveEventListenerV2sByEventAddressIdAsync(
                    eventAddressV2Id, cancellationToken);
    }
}
