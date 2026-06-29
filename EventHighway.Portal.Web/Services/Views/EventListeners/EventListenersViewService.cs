// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.EventListeners;

namespace EventHighway.Portal.Web.Services.Views.EventListeners
{
    public partial class EventListenersViewService : IEventListenersViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventListenersViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventListenerView>> RetrieveListenersByAddressAsync(
            Guid eventAddressId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IQueryable<EventListenerV2> listeners =
                await this.eventHighwayBroker
                    .RetrieveEventListenerV2sByEventAddressIdAsync(
                        eventAddressId, cancellationToken);

            return listeners.Select(AsView).ToList();
        });

        private static EventListenerView AsView(EventListenerV2 listener) =>
            new EventListenerView
            {
                Id = listener.Id,
                Name = listener.Name,
                Description = listener.Description,
                HandlerName = listener.HandlerName,
                EventAddressId = listener.EventAddressId,
                ParticipantId = listener.ParticipantId
            };
    }
}
