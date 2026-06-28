// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using Bunit;
using EventHighway.Portal.Web.Components.Navigation;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Navigation
{
    public partial class NavMenuComponentTests
    {
        [Fact]
        public void ShouldRenderDashboardLinkForAnonymousUser()
        {
            // given
            AddAuthorization();

            // when
            IRenderedComponent<NavMenu> renderedNavMenu = Render<NavMenu>();

            // then
            renderedNavMenu.FindAll("a")
                .Select(anchor => anchor.GetAttribute("href"))
                .Should().Contain("");

            renderedNavMenu.Markup.Should().Contain("Dashboard");
        }
    }
}
