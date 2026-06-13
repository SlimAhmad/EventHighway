// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<EventListenerV2> RegisterEventListenerV2Async(EventListenerV2 eventListenerV2) =>
            await this.eventHighwayClient.V2.EventListenerV2Client.RegisterEventListenerV2Async(eventListenerV2);

        public async ValueTask<IQueryable<EventListenerV2>> RetrieveEventListenerV2sByEventAddressIdAsync(
            Guid eventAddressId) =>
                await this.eventHighwayClient.V2.EventListenerV2Client
                    .RetrieveEventListenerV2sByEventAddressIdAsync(eventAddressId);

        public async ValueTask<EventListenerV2> RemoveEventListenerV2ByIdAsync(Guid eventListenerV2Id) =>
            await this.eventHighwayClient.V2.EventListenerV2Client.RemoveEventListenerV2ByIdAsync(eventListenerV2Id);
    }
}
