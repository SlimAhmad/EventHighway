// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask ArchiveEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask PurgeEventArchiveV2sAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default);
    }
}
