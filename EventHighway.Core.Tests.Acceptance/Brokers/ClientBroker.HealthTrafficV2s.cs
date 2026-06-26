// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Core.Tests.Acceptance.Brokers
{
    public partial class ClientBroker
    {
        public async ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart) =>
            await this.eventHighwayClient.V2.HealthTrafficClientV2
                .RetrieveTrafficSnapshotV2Async(period, windowStart);
    }
}
