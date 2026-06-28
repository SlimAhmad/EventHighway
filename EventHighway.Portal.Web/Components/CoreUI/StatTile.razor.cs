// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Components.CoreUI
{
    public partial class StatTile
    {
        [Parameter]
        public StatTileVariant Variant { get; set; } = StatTileVariant.Na;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string? Icon { get; set; }

        public string VariantCssClass =>
            Variant switch
            {
                StatTileVariant.Green => "text-bg-success",
                StatTileVariant.Amber => "text-bg-warning",
                StatTileVariant.Red => "text-bg-danger",
                _ => "text-bg-secondary"
            };
    }
}
