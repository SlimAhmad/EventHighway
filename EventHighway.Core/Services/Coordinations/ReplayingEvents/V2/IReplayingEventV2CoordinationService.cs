// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Services.Coordinations.ReplayingEvents.V2
{
    internal interface IReplayingEventV2CoordinationService
    {
        ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            CancellationToken cancellationToken = default);

        ValueTask ProcessReplayedListenerEventV2sAsync(
            CancellationToken cancellationToken = default);
    }
}
