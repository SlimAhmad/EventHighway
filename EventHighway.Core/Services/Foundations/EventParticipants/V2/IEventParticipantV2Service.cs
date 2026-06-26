// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Services.Foundations.EventParticipants.V2
{
    internal interface IEventParticipantV2Service
    {
        ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
            CancellationToken cancellationToken = default);
    }
}
