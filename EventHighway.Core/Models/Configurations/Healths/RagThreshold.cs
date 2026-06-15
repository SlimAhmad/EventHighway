// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Configurations.Healths
{
    /// <summary>
    /// Defines the Red/Amber/Green boundary values for a single <see cref="HealthMetric"/>.
    /// The direction of the scale is inferred from the relationship between
    /// <see cref="Green"/> and <see cref="Red"/>: when <c>Green &lt; Red</c> a lower value
    /// is healthier (e.g. dead events); when <c>Green &gt; Red</c> a higher value is
    /// healthier (e.g. handler count).
    /// </summary>
    public class RagThreshold
    {
        /// <summary>
        /// Gets or sets the metric this threshold applies to.
        /// </summary>
        public HealthMetric Metric { get; set; }

        /// <summary>
        /// Gets or sets the boundary value at or beyond which the metric is considered Green (healthy).
        /// </summary>
        public decimal Green { get; set; }

        /// <summary>
        /// Gets or sets the boundary value at or beyond which the metric is considered Red (critical).
        /// Values between <see cref="Green"/> and <see cref="Red"/> are reported as Amber.
        /// </summary>
        public decimal Red { get; set; }
    }
}
