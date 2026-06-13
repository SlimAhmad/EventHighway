// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<EventAddressV2> RegisterEventAddressV2Async(EventAddressV2 eventAddressV2) =>
            await this.eventHighwayClient.V2.EventAddressV2Client.RegisterEventAddressV2Async(eventAddressV2);

        public async ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync() =>
            await this.eventHighwayClient.V2.EventAddressV2Client.RetrieveAllEventAddressV2sAsync();

        public async ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(Guid eventAddressV2Id) =>
            await this.eventHighwayClient.V2.EventAddressV2Client.RemoveEventAddressV2ByIdAsync(eventAddressV2Id);
    }
}
