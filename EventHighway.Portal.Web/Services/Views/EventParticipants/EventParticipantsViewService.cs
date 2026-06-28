// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.EventParticipants;

namespace EventHighway.Portal.Web.Services.Views.EventParticipants
{
    public partial class EventParticipantsViewService : IEventParticipantsViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantsViewService(
            IEventHighwayBroker eventHighwayBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<List<EventParticipantView>> RetrieveAllParticipantsAsync(
            CancellationToken cancellationToken = default)
        {
            IEnumerable<EventParticipantV2> participants =
                await this.eventHighwayBroker.RetrieveAllEventParticipantV2sAsync(
                    cancellationToken);

            return participants.Select(AsView).ToList();
        }

        private static EventParticipantView AsView(EventParticipantV2 participant) =>
            new EventParticipantView
            {
                Id = participant.Id,
                Name = participant.Name,
                Description = participant.Description,
                ContactEmail = participant.ContactEmail,
                ContactPhone = participant.ContactPhone,
                IsActive = participant.IsActive,
                ActiveFrom = participant.ActiveFrom,
                ActiveTo = participant.ActiveTo
            };
    }
}
