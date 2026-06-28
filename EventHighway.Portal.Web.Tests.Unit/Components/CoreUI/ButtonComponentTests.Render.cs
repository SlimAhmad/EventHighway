// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class ButtonComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<Button> renderedButton =
                Render<Button>();

            // then
            renderedButton.Instance.Color.Should().Be("primary");
            renderedButton.Instance.Type.Should().Be("button");
            renderedButton.Instance.Disabled.Should().BeFalse();
        }
    }
}
