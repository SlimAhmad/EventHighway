// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents the current distribution of remaining retry attempts across active events,
    /// used to drive the retry health histogram. Reflects current state only (no window).
    /// </summary>
    public class RetryHealthSummaryV2
    {
        /// <summary>
        /// Gets or sets the total number of active events considered.
        /// </summary>
        public long TotalActiveEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events with zero remaining retry attempts.
        /// </summary>
        public long DeadEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events with one or two remaining retry attempts.
        /// </summary>
        public long CriticalEvents { get; set; }

        /// <summary>
        /// Gets or sets the number of events with three or more remaining retry attempts.
        /// </summary>
        public long HealthyEvents { get; set; }

        /// <summary>
        /// Gets or sets the histogram buckets keyed by remaining retry count.
        /// </summary>
        public IEnumerable<RetryBucketV2> Distribution { get; set; }

        /// <summary>
        /// Gets or sets the per-address retry breakdown.
        /// </summary>
        public IEnumerable<RetryAddressDetailV2> ByAddress { get; set; }
    }
}
