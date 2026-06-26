// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class DuplicateDetectionSummaryV2
    {
        public TrafficPeriodV2 Period { get; set; }
        public DateTimeOffset WindowStart { get; set; }
        public DateTimeOffset WindowEnd { get; set; }
        public string WindowLabel { get; set; }
        public long TotalDuplicatesDetected { get; set; }
        public long TotalUniqueEvents { get; set; }
        public decimal OverallDuplicateRate { get; set; }
        public IEnumerable<DuplicateDetailV2> ByAddress { get; set; }
    }
}
