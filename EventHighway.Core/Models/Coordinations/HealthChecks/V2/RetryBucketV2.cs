// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class RetryBucketV2
    {
        public int RemainingRetries { get; set; }
        public long Count { get; set; }
    }
}
