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

        [Fact]
        public void ShouldRenderButtonWithColorTypeAndChildContent()
        {
            // given
            string randomLabel = GetRandomString();

            // when
            IRenderedComponent<Button> renderedButton =
                Render<Button>(parameters =>
                    parameters
                        .Add(button => button.Color, "success")
                        .Add(button => button.Type, "submit")
                        .AddChildContent(randomLabel));

            // then
            var buttonElement = renderedButton.Find("button");

            buttonElement.ClassList.Should().Contain(new[] { "btn", "btn-success" });
            buttonElement.GetAttribute("type").Should().Be("submit");
            buttonElement.TextContent.Trim().Should().Be(randomLabel);
        }
    }
}
