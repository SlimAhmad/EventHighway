// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the duplicate-detection roll-up for a single period window, derived from
    /// content-hash collisions, with a per-address breakdown.
    /// </summary>
    public class DuplicateDetectionSummaryV2
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
        /// Gets or sets the total number of events detected as content-hash duplicates within the window.
        /// </summary>
        public long TotalDuplicatesDetected { get; set; }

        /// <summary>
        /// Gets or sets the total number of unique events (distinct content hashes) within the window.
        /// </summary>
        public long TotalUniqueEvents { get; set; }

        /// <summary>
        /// Gets or sets the overall percentage of events detected as duplicates within the window.
        /// </summary>
        public decimal OverallDuplicateRate { get; set; }

        /// <summary>
        /// Gets or sets the per-address duplicate breakdown.
        /// </summary>
        public IEnumerable<DuplicateDetailV2> ByAddress { get; set; }
    }
}
