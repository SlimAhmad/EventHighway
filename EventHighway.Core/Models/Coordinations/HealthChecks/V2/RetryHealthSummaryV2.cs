// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class RetryHealthSummaryV2
    {
        public long TotalActiveEvents { get; set; }
        public long DeadEvents { get; set; }
        public long CriticalEvents { get; set; }
        public long HealthyEvents { get; set; }
        public IEnumerable<RetryBucketV2> Distribution { get; set; }
        public IEnumerable<RetryAddressDetailV2> ByAddress { get; set; }
    }
}
