// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class LoopDetailV2
    {
        public Guid EventAddressId { get; set; }
        public string AddressName { get; set; }
        public long ActiveQuarantined { get; set; }
        public long ArchivedQuarantined { get; set; }
        public long InWindow { get; set; }
        public DateTimeOffset? MostRecentDetection { get; set; }
        public HealthStatusV2 Status { get; set; }
    }
}
