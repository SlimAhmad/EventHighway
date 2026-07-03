// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the duplicate-detection breakdown for a single event address within a
    /// <see cref="DuplicateDetectionSummaryV2"/>, including the publishing participant when known.
    /// </summary>
    public class DuplicateDetailV2
    {
        /// <summary>
        /// Gets or sets the identifier of the event address.
        /// </summary>
        public Guid EventAddressId { get; set; }

        /// <summary>
        /// Gets or sets the name of the event address.
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the publishing participant, or <c>null</c> when no
        /// participant is registered for the events.
        /// </summary>
        public Guid? ParticipantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the publishing participant, or "Unknown" when
        /// <see cref="ParticipantId"/> is <c>null</c>.
        /// </summary>
        public string ParticipantName { get; set; }

        /// <summary>
        /// Gets or sets the total number of events on this address within the window.
        /// </summary>
        public long TotalEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events on this address detected as duplicates within the window.
        /// </summary>
        public long Duplicates { get; set; }

        /// <summary>
        /// Gets or sets the percentage of events on this address detected as duplicates within the window.
        /// </summary>
        public decimal DuplicateRate { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the most recent duplicate seen on this address, or
        /// <c>null</c> when none occurred within the window.
        /// </summary>
        public DateTimeOffset? LastDuplicateSeen { get; set; }
    }
}
