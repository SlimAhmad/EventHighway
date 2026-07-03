// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.EventFirings.V2
{
    internal interface IEventFiringV2OrchestrationService
    {
        ValueTask<EventV2> FireEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);
    }
}
