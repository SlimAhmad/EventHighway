// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<IEnumerable<EventAddressSummaryV2>> RetrieveEventAddressSummaryV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthAddressClientV2
                .RetrieveEventAddressSummaryV2Async(period, windowStart);
    }
}
