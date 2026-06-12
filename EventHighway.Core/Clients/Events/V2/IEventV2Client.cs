// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Clients.Events.V2
{
    public interface IEventV2Client
    {
        ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask FireScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default);
    }
}
