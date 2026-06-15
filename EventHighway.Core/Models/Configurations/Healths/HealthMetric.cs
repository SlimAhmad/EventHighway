// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Configurations.Healths
{
    /// <summary>
    /// Identifies the aspect of the EventHighway system that a <see cref="RagThreshold"/> applies to.
    /// </summary>
    public enum HealthMetric
    {
        /// <summary>
        /// The count of events whose remaining retry attempts have reached zero.
        /// </summary>
        DeadEvents,

        /// <summary>
        /// The percentage of listener events that ended in an error state.
        /// </summary>
        ErrorRate,

        /// <summary>
        /// The number of event handlers currently registered with the system.
        /// </summary>
        HandlerCount
    }
}
