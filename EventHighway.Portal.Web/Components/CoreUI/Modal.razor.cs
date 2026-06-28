// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Components;

namespace EventHighway.Portal.Web.Components.CoreUI
{
    public partial class Modal
    {
        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public RenderFragment? FooterContent { get; set; }
    }
}
