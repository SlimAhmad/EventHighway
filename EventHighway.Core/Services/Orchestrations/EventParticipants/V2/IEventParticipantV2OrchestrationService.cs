// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.EventParticipants.V2
{
    internal interface IEventParticipantV2OrchestrationService
    {
        ValueTask ValidateEventParticipantsAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);
    }
}
