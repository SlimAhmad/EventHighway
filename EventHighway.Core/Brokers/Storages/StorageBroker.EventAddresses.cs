// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses;
using Microsoft.EntityFrameworkCore;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial class StorageBroker
    {
        public DbSet<EventAddress> EventAddresses { get; set; }

        public async ValueTask<EventAddress> InsertEventAddressAsync(
            EventAddress eventAddress,
            CancellationToken cancellationToken = default) =>
            await InsertAsync(eventAddress, cancellationToken);

        public async ValueTask<IQueryable<EventAddress>> SelectAllEventAddressesAsync(
            CancellationToken cancellationToken = default) =>
            await SelectAllAsync<EventAddress>(cancellationToken);

        public async ValueTask<EventAddress> SelectEventAddressByIdAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default) =>
            await SelectAsync<EventAddress>(new object[] { eventAddressId }, cancellationToken);
    }
}
