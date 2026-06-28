// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Clients.ArchivingEvents.V2;
using EventHighway.Core.Clients.EventAddresses.V2;
using EventHighway.Core.Clients.EventArchives.V2;
using EventHighway.Core.Clients.EventListeners.V2;
using EventHighway.Core.Clients.EventParticipantSecrets.V2;
using EventHighway.Core.Clients.EventParticipants.V2;
using EventHighway.Core.Clients.Events.V2;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Clients.ListenerEvents.V2;
using EventHighway.Core.Clients.ReplayingEvents.V2;

namespace EventHighway.Core.Clients.EventHighways.V2
{
    /// <summary>
    /// Defines the V2 API contract for the EventHighway client, providing access to event
    /// management operations including event archiving, addresses, listeners, events, health
    /// checks, and listener events.
    /// </summary>
    public interface IClientV2
    {
        /// <summary>
        /// Registers an event handler with the EventHighway V2 client. This method supports
        /// method chaining by returning the current instance.
        /// </summary>
        /// <param name="eventHandler">The event handler to register.</param>
        /// <returns>The current <see cref="IClientV2"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when eventHandler is null.</exception>
        IClientV2 RegisterEventHandler(IEventHandler eventHandler);

        /// <summary>
        /// Gets the client for managing archived events in V2 API.
        /// </summary>
        IArchivingEventV2Client ArchivingEventV2Client { get; }

        /// <summary>
        /// Gets the client for retrieving archived events in V2 API.
        /// </summary>
        IEventArchiveV2Client EventArchiveV2Client { get; }

        /// <summary>
        /// Gets the client for managing event addresses in V2 API.
        /// </summary>
        IEventAddressV2Client EventAddressV2Client { get; }

        /// <summary>
        /// Gets the client for managing event listeners in V2 API.
        /// </summary>
        IEventListenerV2Client EventListenerV2Client { get; }

        /// <summary>
        /// Gets the client for managing events in V2 API.
        /// </summary>
        IEventV2Client EventV2Client { get; }

        /// <summary>
        /// Gets the client for performing health checks in V2 API.
        /// </summary>
        IHealthStatusClientV2 HealthStatusClientV2 { get; }

        /// <summary>
        /// Gets the client for retrieving health traffic snapshots in V2 API.
        /// </summary>
        IHealthTrafficClientV2 HealthTrafficClientV2 { get; }

        /// <summary>
        /// Gets the client for retrieving per-event-address health summaries in V2 API.
        /// </summary>
        IHealthAddressClientV2 HealthAddressClientV2 { get; }

        /// <summary>
        /// Gets the client for retrieving the loop-detection summary in V2 API.
        /// </summary>
        IHealthLoopClientV2 HealthLoopClientV2 { get; }

        /// <summary>
        /// Gets the client for retrieving the duplicate-detection summary in V2 API.
        /// </summary>
        IHealthDuplicateClientV2 HealthDuplicateClientV2 { get; }

        /// <summary>
        /// Gets the client for retrieving the retry-health summary in V2 API.
        /// </summary>
        IHealthRetryClientV2 HealthRetryClientV2 { get; }

        /// <summary>
        /// Gets the client for retrieving the per-participant health summary in V2 API.
        /// </summary>
        IHealthParticipantClientV2 HealthParticipantClientV2 { get; }

        /// <summary>
        /// Gets the client for managing listener events in V2 API.
        /// </summary>
        IListenerEventV2Client ListenerEventV2Client { get; }

        /// <summary>
        /// Gets the client for replaying archived events in V2 API.
        /// </summary>
        IReplayingEventV2Client ReplayingEventV2Client { get; }

        /// <summary>
        /// Gets the client for managing event participants in V2 API.
        /// </summary>
        IEventParticipantV2Client EventParticipantV2Client { get; }

        /// <summary>
        /// Gets the client for managing event participant secrets in V2 API.
        /// </summary>
        IEventParticipantSecretV2Client EventParticipantSecretV2Client { get; }
    }
}
