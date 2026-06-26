// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        DbSet<EventParticipantSecretV2> EventParticipantSecretV2s { get; set; }

        ValueTask<EventParticipantSecretV2> InsertEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventParticipantSecretV2>> SelectAllEventParticipantSecretV2sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> SelectEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> UpdateEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> DeleteEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default);
    }
}
