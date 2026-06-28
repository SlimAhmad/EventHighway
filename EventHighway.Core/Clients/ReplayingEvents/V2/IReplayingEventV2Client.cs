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
    /// Defines the contract for the V2 replaying event client, providing operations to restore
    /// archived events into the replay queue and to deliver queued replay events to their listeners.
    /// </summary>
    public interface IReplayingEventV2Client
    {
        /// <summary>
        /// Restores archived events matching the supplied filters into the replay delivery queue.
        /// Use the optional parameters to narrow the scope by address, specific listeners, and
        /// date window.
        /// </summary>
        /// <param name="eventAddressId">Optional event address identifier to narrow the scope
        /// to a specific address. Pass <c>null</c> to include all addresses.</param>
        /// <param name="eventListenerIds">Optional collection of listener identifiers to target.
        /// Pass <c>null</c> or an empty collection to include all listeners, including any that
        /// were not registered when the original events were published.</param>
        /// <param name="startDate">Optional inclusive start of the date window for archived events.
        /// Pass <c>null</c> to start from the earliest available archive.</param>
        /// <param name="endDate">Optional inclusive end of the date window for archived events.
        /// Pass <c>null</c> to include up to the latest available archive.</param>
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
        /// Restores a single archived event, identified by <paramref name="eventV2Id"/>, into the
        /// replay delivery queue for the supplied listeners. Use this to re-send a specific archived
        /// event (for example a loop-detected event) to a targeted set of listeners.
        /// </summary>
        /// <param name="eventV2Id">The identifier of the archived event to replay.</param>
        /// <param name="eventAddressId">Optional event address identifier. When supplied, the event
        /// is only replayed if its address matches; pass <c>null</c> to skip the address check.</param>
        /// <param name="eventListenerIds">The listener identifiers the event should be replayed to.</param>
        /// <param name="allowReplayOfQuarantinedItem">When <c>true</c>, a quarantined (for example
        /// loop-detected) archived event is eligible for replay. When <c>false</c> (the default),
        /// quarantined events are skipped. Replayed events are not re-evaluated for loop detection.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask ReplayEventArchiveV2sAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delivers all pending replay listener events to their event handlers in batches.
        /// Intended to be called from a recurring background job.
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
