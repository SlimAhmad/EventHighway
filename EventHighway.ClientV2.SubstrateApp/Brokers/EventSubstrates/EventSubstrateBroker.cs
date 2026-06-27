// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public sealed class EventSubstrateBroker : IEventSubstrateBroker
    {
        private readonly EventHighwayClient eventHighwayClient;

        public EventSubstrateBroker(
            string connectionString,
            EventHighwayConfiguration configuration) =>
            this.eventHighwayClient = new EventHighwayClient(connectionString, configuration);

        public IEventSubstrateBroker RegisterEventHandler(IEventHandler eventHandler)
        {
            this.eventHighwayClient.V2.RegisterEventHandler(eventHandler);

            return this;
        }

        public ValueTask<EventParticipantV2> AddParticipantAsync(
            EventParticipantV2 participant,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventParticipantV2Client
                .AddEventParticipantV2Async(participant, cancellationToken);

        public ValueTask<EventParticipantSecretV2> AddParticipantSecretAsync(
            EventParticipantSecretV2 participantSecret,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventParticipantSecretV2Client
                .AddEventParticipantSecretV2Async(participantSecret, cancellationToken);

        public ValueTask<EventAddressV2> RetrieveOrRegisterAddressAsync(
            EventAddressV2 eventAddress,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventAddressV2Client
                .RetrieveOrRegisterEventAddressV2Async(eventAddress, cancellationToken);

        public ValueTask<EventListenerV2> RegisterListenerAsync(
            EventListenerV2 eventListener,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventListenerV2Client
                .RegisterEventListenerV2Async(eventListener, cancellationToken);

        public ValueTask<EventV2> SubmitEventAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventV2Client
                .SubmitEventV2Async(eventV2, cancellationToken);

        public ValueTask FirePendingEventsAsync(CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.EventV2Client
                .FireScheduledPendingEventV2sAsync(cancellationToken);

        public ValueTask ArchiveEventsAsync(CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ArchivingEventV2Client
                .ArchiveEventV2sAsync(cancellationToken);

        public ValueTask ReplayEventToListenersAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ReplayingEventV2Client
                .ReplayEventArchiveV2sAsync(
                    eventV2Id,
                    eventAddressId,
                    eventListenerIds,
                    allowReplayOfQuarantinedItem,
                    cancellationToken);

        public ValueTask ProcessReplayedEventsAsync(CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ReplayingEventV2Client
                .ProcessReplayedListenerEventV2sAsync(cancellationToken);

        public ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventsAsync(
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.ListenerEventV2Client
                .RetrieveAllListenerEventV2sAsync(cancellationToken);

        public ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusAsync(
            CancellationToken cancellationToken = default) =>
            this.eventHighwayClient.V2.HealthStatusClientV2
                .RetrieveHealthRagStatusV2Async(cancellationToken);
    }
}
