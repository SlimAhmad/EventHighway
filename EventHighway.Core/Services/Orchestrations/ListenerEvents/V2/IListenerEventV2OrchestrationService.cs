// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ListenerEvents.V2
{
    public interface IListenerEventV2OrchestrationService
    {
        ValueTask<IEnumerable<ListenerEventV2>> RetrieveBatchOfListenerEventV2sByEventIdsAsync(
            IEnumerable<Guid> eventV2Ids,
            int take,
            CancellationToken cancellationToken = default);

        ValueTask BulkRemoveListenerEventV2sAsync(
            IEnumerable<ListenerEventV2> listenerEventV2s,
            CancellationToken cancellationToken = default);
    }
}
