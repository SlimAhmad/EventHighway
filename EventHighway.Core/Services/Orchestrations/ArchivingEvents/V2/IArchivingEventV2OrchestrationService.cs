// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2
{
    public interface IArchivingEventV2OrchestrationService
    {
        IAsyncEnumerable<EventV2> RetrieveAllDeadEventV2sWithListenersAsync(
            CancellationToken cancellationToken = default);

        ValueTask RemoveEventV2AndListenerEventV2sAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);
    }
}
