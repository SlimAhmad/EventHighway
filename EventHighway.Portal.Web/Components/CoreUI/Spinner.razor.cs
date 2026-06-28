// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Components.CoreUI
{
    public partial class Spinner
    {
        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public string Label { get; set; } = "Loading...";
    }
}
