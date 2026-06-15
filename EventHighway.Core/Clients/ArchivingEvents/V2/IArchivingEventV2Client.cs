// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Clients.ArchivingEvents.V2
{
    /// <summary>
    /// Defines the contract for the V2 archiving event client, providing operations to archive
    /// dead events.
    /// </summary>
    public interface IArchivingEventV2Client
    {
        /// <summary>
        /// Archives dead events asynchronously. This operation processes and moves dead events
        /// to an archive location.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default);
    }
}
