// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<EventV2> SubmitEventV2Async(EventV2 eventV2) =>
            await this.eventHighwayClient.V2.EventV2Client.SubmitEventV2Async(eventV2);

        public async ValueTask FireScheduledPendingEventV2sAsync() =>
            await this.eventHighwayClient.V2.EventV2Client.FireScheduledPendingEventV2sAsync();

        public async ValueTask<EventV2> RemoveEventV2ByIdAsync(Guid eventV2Id) =>
            await this.eventHighwayClient.V2.EventV2Client.RemoveEventV2ByIdAsync(eventV2Id);
    }
}
