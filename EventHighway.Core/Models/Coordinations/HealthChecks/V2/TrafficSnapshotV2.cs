// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class TrafficSnapshotV2
    {
        public TrafficPeriodV2 Period { get; set; }
        public DateTimeOffset WindowStart { get; set; }
        public DateTimeOffset WindowEnd { get; set; }
        public string WindowLabel { get; set; }
        public long TotalEvents { get; set; }
        public long TotalListenerEvents { get; set; }
        public long TotalSuccess { get; set; }
        public long TotalErrors { get; set; }
        public long TotalPending { get; set; }
        public long TotalReplays { get; set; }
        public IEnumerable<TrafficBucketV2> Buckets { get; set; }
    }
}
