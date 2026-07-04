// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Configurations.Retries
{
    /// <summary>
    /// Configures durable, spaced-out retry of failed listener event deliveries.
    /// </summary>
    public class RetryConfiguration
    {
        /// <summary>
        /// Gets or sets the retry budget granted to a delivery on dispatch and added on each extend.
        /// Also the Fibonacci ceiling. Default 15 → ~12.3 h of retrying before dead, with the last few
        /// per-attempt delays capped at <see cref="RetryBackoffMaxMinutes"/>.
        /// </summary>
        public int RetryAttemptsAllowed { get; set; } = 15;

        /// <summary>
        /// Gets or sets the upper cap on any single backoff delay (guards extended items whose Fibonacci
        /// index keeps climbing). Default 180 (3 h).
        /// </summary>
        public int RetryBackoffMaxMinutes { get; set; } = 180;

        /// <summary>
        /// Gets or sets the grace window after the last dispatch before an exhausted delivery may archive.
        /// Default 180 (3 h).
        /// </summary>
        public int DeadAfterMinutes { get; set; } = 180;
    }
}
