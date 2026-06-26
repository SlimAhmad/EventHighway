// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class RetryAddressDetailV2
    {
        public Guid EventAddressId { get; set; }
        public string AddressName { get; set; }
        public long DeadEvents { get; set; }
        public long CriticalEvents { get; set; }
        public long TotalEvents { get; set; }
    }
}
