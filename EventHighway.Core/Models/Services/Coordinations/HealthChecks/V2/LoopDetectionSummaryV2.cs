// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the loop-detection roll-up for a single period window, summarising
    /// quarantined active and archived events with a per-address breakdown.
    /// </summary>
    public class LoopDetectionSummaryV2
    {
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
        /// Gets or sets the total number of quarantined active events within the window.
        /// </summary>
        public long TotalActiveQuarantined { get; set; }

        /// <summary>
        /// Gets or sets the total number of quarantined archived events within the window.
        /// </summary>
        public long TotalArchivedQuarantined { get; set; }

        /// <summary>
        /// Gets or sets the total number of quarantined events (active and archived) within the window.
        /// </summary>
        public long TotalInWindow { get; set; }

        /// <summary>
        /// Gets or sets the per-address quarantine breakdown.
        /// </summary>
        public IEnumerable<LoopDetailV2> ByAddress { get; set; }
    }
}
