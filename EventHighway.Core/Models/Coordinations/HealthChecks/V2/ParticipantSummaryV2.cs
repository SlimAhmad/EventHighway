// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class ParticipantSummaryV2
    {
        public Guid ParticipantId { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool IsActive { get; set; }
        public TrafficPeriodV2 Period { get; set; }
        public DateTimeOffset WindowStart { get; set; }
        public DateTimeOffset WindowEnd { get; set; }
        public string WindowLabel { get; set; }
        public long TotalEventsSubmitted { get; set; }
        public long ActiveEventAddresses { get; set; }
        public IEnumerable<string> ActiveEventAddressNames { get; set; }
        public long TotalListenerEvents { get; set; }
        public long OwnedListeners { get; set; }
        public decimal PublisherErrorRate { get; set; }
        public decimal ListenerErrorRate { get; set; }
        public long LoopsDetected { get; set; }
        public long DuplicatesDetected { get; set; }
        public HealthStatusV2 Status { get; set; }
        public DateTimeOffset? LastActivity { get; set; }
    }
}
