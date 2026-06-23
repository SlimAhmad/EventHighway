// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Services.Orchestrations.RestoringEvents.V2
{
    internal interface IRestoringEventV2OrchestrationService
    {
        ValueTask RestoreAsync(
            IEnumerable<EventArchiveV2> eventArchiveV2s,
            IEnumerable<ListenerEventArchiveV2> listenerEventArchiveV2s,
            CancellationToken cancellationToken = default);
    }
}
