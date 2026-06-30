// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.ListenerEventArchives;

namespace EventHighway.Portal.Web.Services.Views.ListenerEventArchives
{
    public partial class ListenerEventArchivesViewService : IListenerEventArchivesViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public ListenerEventArchivesViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<ListenerEventArchiveView>> RetrieveAllListenerEventArchivesAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<ListenerEventArchiveV2> listenerEventArchives =
                await this.eventHighwayBroker.RetrieveAllListenerEventArchiveV2sAsync(
                    cancellationToken);

            return listenerEventArchives
                .OrderByDescending(listenerEventArchive => listenerEventArchive.CreatedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<List<ListenerEventArchiveView>>
            RetrieveListenerEventArchivesByEventArchiveIdAsync(
                Guid eventArchiveId,
                CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<ListenerEventArchiveV2> listenerEventArchives =
                await this.eventHighwayBroker.RetrieveAllListenerEventArchiveV2sAsync(
                    cancellationToken);

            return listenerEventArchives
                .Where(listenerEventArchive =>
                    listenerEventArchive.EventArchiveV2Id == eventArchiveId)
                .OrderByDescending(listenerEventArchive => listenerEventArchive.CreatedDate)
                .Select(AsView)
                .ToList();
        });

        public ValueTask<ListenerEventArchiveView> RetrieveListenerEventArchiveByIdAsync(
            Guid listenerEventArchiveId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<ListenerEventArchiveV2> listenerEventArchives =
                await this.eventHighwayBroker.RetrieveAllListenerEventArchiveV2sAsync(
                    cancellationToken);

            ListenerEventArchiveV2 listenerEventArchive = listenerEventArchives
                .FirstOrDefault(retrievedArchive => retrievedArchive.Id == listenerEventArchiveId);

            return listenerEventArchive is null ? null : AsView(listenerEventArchive);
        });

        private static ListenerEventArchiveView AsView(
            ListenerEventArchiveV2 listenerEventArchive) =>
            new ListenerEventArchiveView
            {
                Id = listenerEventArchive.Id,
                Status = listenerEventArchive.Status.ToString(),
                Response = listenerEventArchive.Response,
                ResponseCode = listenerEventArchive.ResponseCode,
                ResponseMessage = listenerEventArchive.ResponseMessage,
                EventId = listenerEventArchive.EventId,
                EventAddressId = listenerEventArchive.EventAddressId,
                EventListenerId = listenerEventArchive.EventListenerId,
                EventArchiveV2Id = listenerEventArchive.EventArchiveV2Id,
                ParticipantId = listenerEventArchive.ParticipantId,
                CreatedDate = listenerEventArchive.CreatedDate,
                ArchivedDate = listenerEventArchive.ArchivedDate
            };
    }
}
