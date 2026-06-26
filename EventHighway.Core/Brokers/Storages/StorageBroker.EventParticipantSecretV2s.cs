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
    internal partial class StorageBroker
    {
        public DbSet<EventParticipantSecretV2> EventParticipantSecretV2s { get; set; }

        public async ValueTask<EventParticipantSecretV2> InsertEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventParticipantSecretV2, cancellationToken);

        public async ValueTask<IQueryable<EventParticipantSecretV2>> SelectAllEventParticipantSecretV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventParticipantSecretV2>(cancellationToken);

        public async ValueTask<EventParticipantSecretV2> SelectEventParticipantSecretV2ByIdAsync(
            Guid eventParticipantSecretV2Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventParticipantSecretV2>(
                new object[] { eventParticipantSecretV2Id },
                cancellationToken);

        public async ValueTask<EventParticipantSecretV2> UpdateEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            await UpdateAsync(eventParticipantSecretV2, cancellationToken);

        public async ValueTask<EventParticipantSecretV2> DeleteEventParticipantSecretV2Async(
            EventParticipantSecretV2 eventParticipantSecretV2,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventParticipantSecretV2, cancellationToken);
    }
}
