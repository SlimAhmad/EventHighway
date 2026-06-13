// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Core.Services.Coordinations.ArchivingEvents.V2
{
    public interface IArchivingEventV2CoordinationService
    {
        ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default);
    }
}
