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
using EventHighway.Portal.Web.Models.Views.EventParticipantSecrets;

namespace EventHighway.Portal.Web.Services.Views.EventParticipantSecrets
{
    public partial class EventParticipantSecretsViewService : IEventParticipantSecretsViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventParticipantSecretsViewService(
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask<List<EventParticipantSecretView>>
            RetrieveSecretsByParticipantAsync(
                Guid participantId,
                CancellationToken cancellationToken = default)
        {
            IEnumerable<EventParticipantSecretV2> secrets =
                await this.eventHighwayBroker.RetrieveAllEventParticipantSecretV2sAsync(
                    cancellationToken);

            return secrets
                .Where(secret => secret.ParticipantId == participantId)
                .Select(AsView)
                .ToList();
        }

        public async ValueTask<EventParticipantSecretView> AddSecretAsync(
            EventParticipantSecretView secret,
            CancellationToken cancellationToken = default)
        {
            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            var secretToAdd = new EventParticipantSecretV2
            {
                Secret = secret.Secret,
                IsActive = secret.IsActive,
                ActiveFrom = secret.ActiveFrom,
                ActiveTo = secret.ActiveTo,
                ParticipantId = secret.ParticipantId,
                CreatedDate = now,
                UpdatedDate = now
            };

            EventParticipantSecretV2 addedSecret =
                await this.eventHighwayBroker.AddEventParticipantSecretV2Async(
                    secretToAdd, cancellationToken);

            return AsView(addedSecret);
        }

        private static EventParticipantSecretView AsView(EventParticipantSecretV2 secret) =>
            new EventParticipantSecretView
            {
                Id = secret.Id,
                Secret = secret.Secret,
                IsActive = secret.IsActive,
                ActiveFrom = secret.ActiveFrom,
                ActiveTo = secret.ActiveTo,
                ParticipantId = secret.ParticipantId
            };
    }
}
