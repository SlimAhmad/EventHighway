// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using EventHighway.Portal.Web.Components.Navigation;
using EventHighway.Portal.Web.Models.Views.Navigations;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Navigation
{
    public class NavMenuProviderTests
    {
        [Fact]
        public void ShouldReturnDashboardAsFirstNavItem()
        {
            // given . when
            IReadOnlyList<NavItem> navMenu = NavMenuProvider.GetNavMenu();

            // then
            navMenu.Should().NotBeEmpty();
            navMenu[0].Title.Should().Be("Dashboard");
            navMenu[0].Href.Should().Be("");
            navMenu[0].Roles.Should().BeNull();
        }
    }
}
