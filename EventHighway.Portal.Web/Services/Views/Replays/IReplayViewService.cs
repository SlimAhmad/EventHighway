// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Models.Views.Replays;

namespace EventHighway.Portal.Web.Services.Views.Replays
{
    public interface IReplayViewService
    {
        ValueTask ReplayAsync(
            ReplayRequestView replayRequest,
            CancellationToken cancellationToken = default);

        ValueTask ReplayListenerEventArchiveAsync(
            Guid eventV2Id,
            Guid eventAddressId,
            Guid eventListenerId,
            CancellationToken cancellationToken = default);
    }
}
