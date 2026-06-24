// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Clients.ReplayingEvents.V2
{
    /// <summary>
    /// Defines the contract for the V2 replaying event client, providing operations to replay
    /// archived events and process queued replay listener events.
    /// </summary>
    public interface IReplayingEventV2Client
    {
        /// <summary>
        /// Replays archived events asynchronously for specified listeners within an optional date
        /// range. This operation restores events from archives and queues them for replay delivery.
        /// </summary>
        /// <param name="eventAddressId">Optional event address identifier to filter replay scope.</param>
        /// <param name="eventListenerIds">Optional collection of listener identifiers to target.</param>
        /// <param name="startDate">Optional start date for the replay window.</param>
        /// <param name="endDate">Optional end date for the replay window.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes queued replay listener events asynchronously in batches until all pending
        /// events have been delivered.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask ProcessReplayedListenerEventV2sAsync(
            CancellationToken cancellationToken = default);
    }
}
