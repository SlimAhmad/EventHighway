// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Coordinations.HealthChecks.V2
{
    public class HealthCheckItemV2
    {
        public string Grouping { get; set; }
        public string Item { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
    }
}
