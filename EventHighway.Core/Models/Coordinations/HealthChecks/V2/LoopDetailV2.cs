// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the loop-detection breakdown for a single event address within a
    /// <see cref="LoopDetectionSummaryV2"/>.
    /// </summary>
    public class LoopDetailV2
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
        /// Gets or sets the number of quarantined active events for this address.
        /// </summary>
        public long ActiveQuarantined { get; set; }

        /// <summary>
        /// Gets or sets the number of quarantined archived events for this address.
        /// </summary>
        public long ArchivedQuarantined { get; set; }

        /// <summary>
        /// Gets or sets the number of quarantined events for this address within the window.
        /// </summary>
        public long InWindow { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the most recent loop detection for this address, or
        /// <c>null</c> when none occurred within the window.
        /// </summary>
        public DateTimeOffset? MostRecentDetection { get; set; }

        /// <summary>
        /// Gets or sets the RAG status computed for this address row.
        /// </summary>
        public HealthStatusV2 Status { get; set; }
    }
}
