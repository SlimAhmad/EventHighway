// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class FormTextComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<FormText> renderedFormText = Render<FormText>();

            // then
            renderedFormText.Instance.Value.Should().BeNull();
            renderedFormText.Find("input").Should().NotBeNull();
        }

        [Fact]
        public void ShouldRenderLabelAndValue()
        {
            // given
            string randomLabel = GetRandomString();
            string randomValue = GetRandomString();

            // when
            IRenderedComponent<FormText> renderedFormText =
                Render<FormText>(parameters => parameters
                    .Add(formText => formText.Label, randomLabel)
                    .Add(formText => formText.Value, randomValue));

            // then
            renderedFormText.Find("label").TextContent.Should().Contain(randomLabel);
            renderedFormText.Find("input").GetAttribute("value").Should().Be(randomValue);
        }
    }
}
