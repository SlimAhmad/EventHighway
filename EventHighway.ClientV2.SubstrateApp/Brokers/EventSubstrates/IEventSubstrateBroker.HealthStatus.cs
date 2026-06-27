// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusAsync(
            CancellationToken cancellationToken = default);
    }
}
