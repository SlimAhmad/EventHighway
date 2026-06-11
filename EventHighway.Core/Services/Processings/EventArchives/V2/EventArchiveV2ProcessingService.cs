// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;

namespace EventHighway.Core.Services.Processings.EventArchives.V2
{
    internal partial class EventArchiveV2ProcessingService : IEventArchiveV2ProcessingService
    {
        private readonly IEventArchiveV2Service eventArchiveV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventArchiveV2ProcessingService(
            IEventArchiveV2Service eventArchiveV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventArchiveV2Service = eventArchiveV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventArchiveV2> AddEventArchiveV2Async(
            EventArchiveV2 eventArchiveV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventArchiveV2IsNotNull(eventArchiveV2);

            return await this.eventArchiveV2Service.AddEventArchiveV2Async(
                eventArchiveV2,
                cancellationToken);
        });
    }
}
