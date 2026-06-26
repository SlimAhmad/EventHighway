// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class EventAddressSummaryV2
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TrafficPeriodV2 Period { get; set; }
        public DateTimeOffset WindowStart { get; set; }
        public DateTimeOffset WindowEnd { get; set; }
        public string WindowLabel { get; set; }
        public long TotalActiveEvents { get; set; }
        public long TotalArchivedEvents { get; set; }
        public long TotalListenerEvents { get; set; }
        public long TotalArchivedListenerEvents { get; set; }
        public long ActiveListeners { get; set; }
        public long DeadEvents { get; set; }
        public long LoopsDetected { get; set; }
        public decimal ErrorRate { get; set; }
        public decimal DuplicateRate { get; set; }
        public HealthStatusV2 Status { get; set; }
        public DateTimeOffset? LastActivity { get; set; }
    }
}
