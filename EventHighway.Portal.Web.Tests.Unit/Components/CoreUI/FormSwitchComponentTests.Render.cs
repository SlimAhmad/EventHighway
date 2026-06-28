// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class FormSwitchComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<FormSwitch> renderedFormSwitch = Render<FormSwitch>();

            // then
            renderedFormSwitch.Instance.Value.Should().BeFalse();
            renderedFormSwitch.Find("input[type=checkbox]").Should().NotBeNull();
        }

        [Fact]
        public void ShouldRenderLabelAndCheckedState()
        {
            // given
            string randomLabel = GetRandomString();

            // when
            IRenderedComponent<FormSwitch> renderedFormSwitch =
                Render<FormSwitch>(parameters => parameters
                    .Add(formSwitch => formSwitch.Label, randomLabel)
                    .Add(formSwitch => formSwitch.Value, true));

            // then
            renderedFormSwitch.Find("label").TextContent.Should().Contain(randomLabel);
            renderedFormSwitch.Find("input[type=checkbox]").HasAttribute("checked")
                .Should().BeTrue();
        }
    }
}
