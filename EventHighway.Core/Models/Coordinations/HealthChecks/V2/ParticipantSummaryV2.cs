// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the per-participant health roll-up for a single period window, combining
    /// publishing activity and listener ownership, used to drive the usage-by-participant table.
    /// </summary>
    public class ParticipantSummaryV2
    {
        /// <summary>
        /// Gets or sets the identifier of the participant.
        /// </summary>
        public Guid ParticipantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the participant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the participant's contact email address.
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the participant's contact phone number.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the participant is currently active.
        /// </summary>
        public bool IsActive { get; set; }

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
        /// Gets or sets the number of events this participant submitted within the window.
        /// </summary>
        public long TotalEventsSubmitted { get; set; }

        /// <summary>
        /// Gets or sets the number of distinct event addresses this participant published to within the window.
        /// </summary>
        public long ActiveEventAddresses { get; set; }

        /// <summary>
        /// Gets or sets the names of the event addresses this participant published to within the window.
        /// </summary>
        public IEnumerable<string> ActiveEventAddressNames { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events on listeners owned by this participant within the window.
        /// </summary>
        public long TotalListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listeners owned by this participant.
        /// </summary>
        public long OwnedListeners { get; set; }

        /// <summary>
        /// Gets or sets the percentage of errors on events this participant submitted.
        /// </summary>
        public decimal PublisherErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the percentage of errors on listeners this participant owns.
        /// </summary>
        public decimal ListenerErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the number of loop detections attributed to this participant within the window.
        /// </summary>
        public long LoopsDetected { get; set; }

        /// <summary>
        /// Gets or sets the number of duplicates attributed to this participant within the window.
        /// </summary>
        public long DuplicatesDetected { get; set; }

        /// <summary>
        /// Gets or sets the overall RAG status computed for this participant row.
        /// </summary>
        public HealthStatusV2 Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the participant's most recent activity, or
        /// <c>null</c> when there was no activity within the window.
        /// </summary>
        public DateTimeOffset? LastActivity { get; set; }
    }
}
