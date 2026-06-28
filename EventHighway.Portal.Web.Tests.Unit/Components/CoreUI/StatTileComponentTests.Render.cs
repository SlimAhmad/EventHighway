// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class StatTileComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<StatTile> renderedStatTile = Render<StatTile>();

            // then
            renderedStatTile.Instance.Variant.Should().Be(StatTileVariant.Na);
            renderedStatTile.Instance.Value.Should().BeNull();
        }
    }
}
