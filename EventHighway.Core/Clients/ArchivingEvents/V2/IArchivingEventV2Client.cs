// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Clients.ArchivingEvents.V2
{
    public interface IArchivingEventV2Client
    {
        ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default);
    }
}
