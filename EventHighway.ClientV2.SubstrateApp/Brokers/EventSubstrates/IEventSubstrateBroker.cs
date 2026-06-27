// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    /// <summary>
    /// Abstracts the EventHighway dependency behind a single broker so the application never
    /// talks to <c>EventHighwayClient</c> directly. Swap this implementation to retarget the app
    /// at a different event substrate without touching the application code.
    /// </summary>
    public interface IEventSubstrateBroker
    {
        IEventSubstrateBroker RegisterEventHandler(IEventHandler eventHandler);

        ValueTask<EventParticipantV2> AddParticipantAsync(
            EventParticipantV2 participant,
            CancellationToken cancellationToken = default);

        ValueTask<EventParticipantSecretV2> AddParticipantSecretAsync(
            EventParticipantSecretV2 participantSecret,
            CancellationToken cancellationToken = default);

        ValueTask<EventAddressV2> RetrieveOrRegisterAddressAsync(
            EventAddressV2 eventAddress,
            CancellationToken cancellationToken = default);

        ValueTask<EventListenerV2> RegisterListenerAsync(
            EventListenerV2 eventListener,
            CancellationToken cancellationToken = default);

        ValueTask<EventV2> SubmitEventAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default);

        ValueTask FirePendingEventsAsync(CancellationToken cancellationToken = default);

        ValueTask ArchiveEventsAsync(CancellationToken cancellationToken = default);

        ValueTask ReplayEventToListenersAsync(
            Guid eventV2Id,
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            bool allowReplayOfQuarantinedItem = false,
            CancellationToken cancellationToken = default);

        ValueTask ProcessReplayedEventsAsync(CancellationToken cancellationToken = default);

        ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventsAsync(
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusAsync(
            CancellationToken cancellationToken = default);
    }
}
