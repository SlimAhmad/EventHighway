// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;

namespace EventHighway.Core.Services.Processings.EventAddresses.V2
{
    internal partial class EventAddressV2ProcessingService : IEventAddressV2ProcessingService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventAddressV2ProcessingService(
            IEventAddressV2Service eventAddressV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync() =>
        TryCatch(async () =>
        {
            return await this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync();
        });

        public ValueTask<EventAddressV2> RetrieveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventAddressV2Id(eventAddressV2Id);

            return await this.eventAddressV2Service.RetrieveEventAddressV2ByIdAsync(
                eventAddressV2Id,
                cancellationToken);
        });

        public async ValueTask<EventAddressV2> RetrieveOrRegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default)
        {
            return await this.eventAddressV2Service.RetrieveEventAddressV2ByIdAsync(
                eventAddressV2.Id,
                cancellationToken);
        }
    }
}
