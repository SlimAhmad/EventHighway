// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async() =>
            await this.eventHighwayClient.V2.HealthRetryClientV2
                .RetrieveRetryHealthV2Async();
    }
}
