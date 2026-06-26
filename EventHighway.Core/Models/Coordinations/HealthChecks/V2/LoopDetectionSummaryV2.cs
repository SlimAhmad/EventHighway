// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class LoopDetectionSummaryV2
    {
        public TrafficPeriodV2 Period { get; set; }
        public DateTimeOffset WindowStart { get; set; }
        public DateTimeOffset WindowEnd { get; set; }
        public string WindowLabel { get; set; }
        public long TotalActiveQuarantined { get; set; }
        public long TotalArchivedQuarantined { get; set; }
        public long TotalInWindow { get; set; }
        public IEnumerable<LoopDetailV2> ByAddress { get; set; }
    }
}
