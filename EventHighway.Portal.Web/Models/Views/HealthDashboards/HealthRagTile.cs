// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Portal.Web.Components.CoreUI;

namespace EventHighway.Portal.Web.Models.Views.HealthDashboards
{
    public class HealthRagTile
    {
        public string Grouping { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public StatTileVariant Variant { get; set; }
    }
}
