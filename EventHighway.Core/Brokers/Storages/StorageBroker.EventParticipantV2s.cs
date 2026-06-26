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
        public DbSet<EventParticipantV2> EventParticipantV2s { get; set; }

        public async ValueTask<EventParticipantV2> InsertEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventParticipantV2, cancellationToken);

        public async ValueTask<IQueryable<EventParticipantV2>> SelectAllEventParticipantV2sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventParticipantV2>(cancellationToken);

        public async ValueTask<EventParticipantV2> SelectEventParticipantV2ByIdAsync(
            Guid eventParticipantV2Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventParticipantV2>(
                new object[] { eventParticipantV2Id },
                cancellationToken);

        public async ValueTask<EventParticipantV2> UpdateEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
            await UpdateAsync(eventParticipantV2, cancellationToken);

        public async ValueTask<EventParticipantV2> DeleteEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventParticipantV2, cancellationToken);
    }
}
