// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public sealed partial class EventHighwayBroker
    {
        public async ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusV2Async(
            CancellationToken cancellationToken = default) =>
            await this.clientV2.HealthStatusClientV2
                .RetrieveHealthRagStatusV2Async(cancellationToken);
    }
}
