// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask ArchiveDeadEventV2sAsync() =>
            await this.eventHighwayClient.V2.ArchivingEventV2Client.ArchiveEventV2sAsync();
    }
}
