// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;

namespace EventHighway.Core.Services.Foundations.EventAddresses.V2
{
    internal partial class EventAddressV2Service : IEventAddressV2Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventAddressV2Service(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventAddressV2> AddEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            await ValidateEventAddressV2OnAddAsync(eventAddressV2);

            return await this.storageBroker.InsertEventAddressV2Async(eventAddressV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return await this.storageBroker.SelectAllEventAddressV2sAsync(cancellationToken);
        });

        public ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<EventAddressV2> RetrieveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventAddressV2Id(eventAddressV2Id);

            return await this.storageBroker.SelectEventAddressV2ByIdAsync(eventAddressV2Id, cancellationToken);
        });

        public ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventAddressV2Id(eventAddressV2Id);

            EventAddressV2 maybeEventAddressV2 =
                await this.storageBroker.SelectEventAddressV2ByIdAsync(eventAddressV2Id, cancellationToken);

            ValidateEventAddressV2Exists(maybeEventAddressV2, eventAddressV2Id);

            return await this.storageBroker.DeleteEventAddressV2Async(maybeEventAddressV2, cancellationToken);
        });
    }
}
