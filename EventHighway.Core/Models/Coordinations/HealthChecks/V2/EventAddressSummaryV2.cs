// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the per-event-address health roll-up for a single period window, used to
    /// drive the usage table and the Traffic and Sales sidebar.
    /// </summary>
    public class EventAddressSummaryV2
    {
        /// <summary>
        /// Gets or sets the identifier of the event address.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the event address.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the event address.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the period granularity the summary was aggregated over.
        /// </summary>
        public TrafficPeriodV2 Period { get; set; }

        /// <summary>
        /// Gets or sets the inclusive UTC start of the window.
        /// </summary>
        public DateTimeOffset WindowStart { get; set; }

        /// <summary>
        /// Gets or sets the exclusive UTC end of the window.
        /// </summary>
        public DateTimeOffset WindowEnd { get; set; }

        /// <summary>
        /// Gets or sets the human-readable label describing the window.
        /// </summary>
        public string WindowLabel { get; set; }

        /// <summary>
        /// Gets or sets the number of active events on this address within the window.
        /// </summary>
        public long TotalActiveEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of archived events on this address within the window.
        /// </summary>
        public long TotalArchivedEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events on this address within the window.
        /// </summary>
        public long TotalListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of archived listener events on this address within the window.
        /// </summary>
        public long TotalArchivedListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listeners currently registered against this address.
        /// </summary>
        public long ActiveListeners { get; set; }

        /// <summary>
        /// Gets or sets the number of events on this address whose remaining retry attempts have reached zero.
        /// </summary>
        public long DeadEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of quarantined (loop-detected) events on this address within the window.
        /// </summary>
        public long LoopsDetected { get; set; }

        /// <summary>
        /// Gets or sets the percentage of listener events on this address that ended in an error state.
        /// </summary>
        public decimal ErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the percentage of events on this address detected as content-hash duplicates.
        /// </summary>
        public decimal DuplicateRate { get; set; }

        /// <summary>
        /// Gets or sets the overall RAG status computed for this address row.
        /// </summary>
        public HealthStatusV2 Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the most recent activity on this address, or
        /// <c>null</c> when there was no activity within the window.
        /// </summary>
        public DateTimeOffset? LastActivity { get; set; }
    }
}
