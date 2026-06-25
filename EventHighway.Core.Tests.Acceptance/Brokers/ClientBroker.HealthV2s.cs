// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthSummaryV2Async() =>
            await this.eventHighwayClient.V2.HealthV2Client.RetrieveHealthSummaryV2Async();
    }
}
