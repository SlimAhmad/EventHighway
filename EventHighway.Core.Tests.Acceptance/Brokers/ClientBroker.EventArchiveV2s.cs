// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync() =>
            await this.eventHighwayClient.V2.EventArchiveV2Client.RetrieveAllEventArchiveV2sAsync();

        public async ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(Guid eventArchiveV2Id) =>
            await this.eventHighwayClient.V2.EventArchiveV2Client
                .RetrieveEventArchiveV2ByIdAsync(eventArchiveV2Id);
    }
}
