// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal interface IEventParticipantSecretV2Service
    {
        ValueTask<EventParticipantSecretV2> AddEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventParticipantSecretV2>> RetrieveAllEventParticipantSecretV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> RetrieveEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default);
    }
}
