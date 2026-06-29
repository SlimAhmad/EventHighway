// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;

namespace EventHighway.Portal.Web.Services.Views.EventArchives
{
    public partial class EventArchivesViewService : IEventArchivesViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventArchivesViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask ArchiveProcessedEventsAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.eventHighwayBroker.ArchiveEventV2sAsync(cancellationToken);
        });

        public ValueTask PurgeArchivesOlderThanAsync(
            DateTimeOffset olderThan,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            await this.eventHighwayBroker.PurgeEventArchiveV2sAsync(
                olderThan, cancellationToken);
        });
    }
}
