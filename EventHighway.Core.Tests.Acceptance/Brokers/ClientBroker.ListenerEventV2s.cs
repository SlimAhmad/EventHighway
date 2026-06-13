// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync() =>
            await this.eventHighwayClient.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

        public async ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(Guid listenerEventV2Id) =>
            await this.eventHighwayClient.V2.ListenerEventV2Client.RemoveListenerEventV2ByIdAsync(listenerEventV2Id);
    }
}
