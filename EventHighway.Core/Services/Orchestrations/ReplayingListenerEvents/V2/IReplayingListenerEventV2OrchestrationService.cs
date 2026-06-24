// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2
{
    internal interface IReplayingListenerEventV2OrchestrationService
    {
        ValueTask<ListenerEventV2> ProcessReplayListenerEventV2Async(
            ListenerEventV2 listenerEventV2,
            CancellationToken cancellationToken = default);
    }
}
