// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V1;

namespace EventHighway.Core.Brokers.Storages
{
    internal partial interface IStorageBroker
    {
        ValueTask<EventAddressV1> InsertEventAddressV1Async(
            EventAddressV1 eventAddressV1,
            CancellationToken cancellationToken = default);

        ValueTask<IQueryable<EventAddressV1>> SelectAllEventAddressV1sAsync(
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressV1> SelectEventAddressV1ByIdAsync(
            Guid eventAddressV1Id,
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressV1> DeleteEventAddressV1Async(
            EventAddressV1 eventAddressV1,
            CancellationToken cancellationToken = default);
    }
}
