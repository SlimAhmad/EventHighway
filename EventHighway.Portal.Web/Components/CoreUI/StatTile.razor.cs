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

        // CoreUI theme colours (bg-success #2eb85c, bg-warning #f9b115, bg-danger #e55353,
        // bg-secondary). text-white forces legible white text on every variant, including warning
        // which Bootstrap would otherwise render with dark text.
        public string VariantCssClass =>
            Variant switch
            {
                StatTileVariant.Green => "bg-success text-white",
                StatTileVariant.Amber => "bg-warning text-white",
                StatTileVariant.Red => "bg-danger text-white",
                _ => "bg-secondary text-white"
            };
    }
}
