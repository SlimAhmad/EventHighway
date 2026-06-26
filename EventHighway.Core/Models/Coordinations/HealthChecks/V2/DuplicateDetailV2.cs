// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class DuplicateDetailV2
    {
        public Guid EventAddressId { get; set; }
        public string AddressName { get; set; }
        public Guid? ParticipantId { get; set; }
        public string ParticipantName { get; set; }
        public long TotalEvents { get; set; }
        public long Duplicates { get; set; }
        public decimal DuplicateRate { get; set; }
        public DateTimeOffset? LastDuplicateSeen { get; set; }
    }
}
