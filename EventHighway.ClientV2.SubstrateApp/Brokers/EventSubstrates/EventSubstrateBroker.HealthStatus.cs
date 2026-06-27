// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed partial class EventSubstrateBroker
    {
        public ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusAsync(
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.HealthStatusClientV2
                .RetrieveHealthRagStatusV2Async(cancellationToken);
    }
}
