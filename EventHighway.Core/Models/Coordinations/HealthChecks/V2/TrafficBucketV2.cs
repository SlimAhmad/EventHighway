// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents a single time bucket within a <see cref="TrafficSnapshotV2"/>, holding the
    /// event and listener-event counts that fall inside the bucket's span.
    /// </summary>
    public class TrafficBucketV2
    {
        /// <summary>
        /// Gets or sets the inclusive UTC start of the bucket's span.
        /// </summary>
        public DateTimeOffset PeriodStart { get; set; }

        /// <summary>
        /// Gets or sets the axis label for the bucket (for example "14:00" or "Mon").
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the total number of events created within the bucket.
        /// </summary>
        public long Events { get; set; }

        /// <summary>
        /// Gets or sets the number of immediate-type events created within the bucket.
        /// </summary>
        public long ImmediateEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of scheduled-type events created within the bucket.
        /// </summary>
        public long ScheduledEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events created within the bucket.
        /// </summary>
        public long ListenerEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events in a successful state within the bucket.
        /// </summary>
        public long Success { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events in an error state within the bucket.
        /// </summary>
        public long Errors { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events still pending within the bucket.
        /// </summary>
        public long Pending { get; set; }

        /// <summary>
        /// Gets or sets the number of listener events in a replay state within the bucket.
        /// </summary>
        public long Replays { get; set; }
    }
}
