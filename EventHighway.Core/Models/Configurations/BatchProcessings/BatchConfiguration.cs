// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Configurations.BatchProcessings
{
    /// <summary>
    /// Configures bulk event processing behaviour for the EventHighway system.
    /// </summary>
    public class BatchConfiguration
    {
        /// <summary>
        /// Gets or sets the number of items processed in a single bulk-processing batch.
        /// A value of <c>0</c> (the default) disables bulk processing.
        /// </summary>
        public int BatchSizeForBulkProcessing { get; set; } = 10000;
    }
}
