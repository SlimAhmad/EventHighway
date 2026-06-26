// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents time-bucketed event and listener-event traffic aggregated over a
    /// single period window, used to drive the dashboard traffic chart and summary chips.
    /// </summary>
    public class TrafficSnapshotV2
    {
        /// <summary>
        /// Gets or sets the period granularity the snapshot was aggregated over.
        /// </summary>
        public TrafficPeriodV2 Period { get; set; }

        /// <summary>
        /// Gets or sets the inclusive UTC start of the window.
        /// </summary>
        public DateTimeOffset WindowStart { get; set; }

        /// <summary>
        /// Gets or sets the exclusive UTC end of the window, derived from
        /// <see cref="Period"/> and <see cref="WindowStart"/>.
        /// </summary>
        public DateTimeOffset WindowEnd { get; set; }

        /// <summary>
        /// Gets or sets the human-readable label describing the window (for example "Jun 2026").
        /// </summary>
        public string WindowLabel { get; set; }

        /// <summary>
        /// Gets or sets the total number of events created within the window.
        /// </summary>
        public long TotalEvents { get; set; }

        /// <summary>
        /// Gets or sets the total number of listener events created within the window.
        /// </summary>
        public long TotalListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events in a successful state within the window.
        /// </summary>
        public long TotalSuccess { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events in an error state within the window.
        /// </summary>
        public long TotalErrors { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events still pending within the window.
        /// </summary>
        public long TotalPending { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events in a replay state within the window.
        /// </summary>
        public long TotalReplays { get; set; }

        /// <summary>
        /// Gets or sets the ordered collection of buckets the window is divided into.
        /// </summary>
        public IEnumerable<TrafficBucketV2> Buckets { get; set; }
    }
}
