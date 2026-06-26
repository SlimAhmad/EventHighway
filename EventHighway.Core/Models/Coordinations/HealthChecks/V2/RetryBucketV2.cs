// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    /// <summary>
    /// Represents a single histogram bucket within a <see cref="RetryHealthSummaryV2"/>,
    /// pairing a remaining-retry count with the number of events that have it.
    /// </summary>
    public class RetryBucketV2
    {
        /// <summary>
        /// Gets or sets the remaining retry attempts this bucket represents.
        /// </summary>
        public int RemainingRetries { get; set; }

        /// <summary>
        /// Gets or sets the number of events with this remaining-retry count.
        /// </summary>
        public long Count { get; set; }
    }
}
