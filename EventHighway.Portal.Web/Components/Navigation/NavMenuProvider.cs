// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Portal.Web.Models.Views.Navigations;

namespace EventHighway.Portal.Web.Components.Navigation
{
    public static class NavMenuProvider
    {
        public static IReadOnlyList<NavItem> GetNavMenu() =>
            Array.Empty<NavItem>();
    }
}
