// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.EventParticipants;

namespace EventHighway.Portal.Web.Services.Views.EventParticipants
{
    public partial class EventParticipantsViewService : IEventParticipantsViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantsViewService(
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventParticipantView>> RetrieveAllParticipantsAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            IEnumerable<EventParticipantV2> participants =
                await this.eventHighwayBroker.RetrieveAllEventParticipantV2sAsync(
                    cancellationToken);

            return participants.Select(AsView).ToList();
        });

        public ValueTask<EventParticipantView> RetrieveParticipantByIdAsync(
            System.Guid participantId,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            EventParticipantV2 participant =
                await this.eventHighwayBroker.RetrieveEventParticipantV2ByIdAsync(
                    participantId, cancellationToken);

            return AsView(participant);
        });

        public ValueTask<EventParticipantView> AddParticipantAsync(
            EventParticipantView participant,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var participantToAdd = new EventParticipantV2
            {
                Name = participant.Name,
                Description = participant.Description,
                ContactEmail = participant.ContactEmail,
                ContactPhone = participant.ContactPhone,
                IsActive = participant.IsActive,
                ActiveFrom = participant.ActiveFrom,
                ActiveTo = participant.ActiveTo,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventParticipantV2 addedParticipant =
                await this.eventHighwayBroker.AddEventParticipantV2Async(
                    participantToAdd, cancellationToken);

            return AsView(addedParticipant);
        });

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
