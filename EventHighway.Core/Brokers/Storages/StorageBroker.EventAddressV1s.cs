// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventAddressV1> EventAddressV1s { get; set; }

        public async ValueTask<EventAddressV1> InsertEventAddressV1Async(
            EventAddressV1 eventAddressV1,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventAddressV1, cancellationToken);

        public async ValueTask<IQueryable<EventAddressV1>> SelectAllEventAddressV1sAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventAddressV1>(cancellationToken);

        public async ValueTask<EventAddressV1> SelectEventAddressV1ByIdAsync(
            Guid eventAddressV1Id,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventAddressV1>(new object[] { eventAddressV1Id }, cancellationToken);

        public async ValueTask<EventAddressV1> DeleteEventAddressV1Async(
            EventAddressV1 eventAddressV1,
            CancellationToken cancellationToken = default) =>
            await DeleteAsync(eventAddressV1, cancellationToken);
    }
}
