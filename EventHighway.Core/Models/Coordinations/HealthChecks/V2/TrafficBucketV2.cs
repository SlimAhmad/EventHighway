// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class TrafficBucketV2
    {
        public DateTimeOffset PeriodStart { get; set; }
        public string Label { get; set; }
        public long Events { get; set; }
        public long ImmediateEvents { get; set; }
        public long ScheduledEvents { get; set; }
        public long ListenerEvents { get; set; }
        public long Success { get; set; }
        public long Errors { get; set; }
        public long Pending { get; set; }
        public long Replays { get; set; }
    }
}
