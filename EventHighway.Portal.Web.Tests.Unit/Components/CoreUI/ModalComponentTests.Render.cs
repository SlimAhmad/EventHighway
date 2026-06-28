// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class ModalComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given . when
            IRenderedComponent<Modal> renderedModal = Render<Modal>();

            // then
            renderedModal.Instance.Visible.Should().BeFalse();
            renderedModal.FindAll("div.modal").Should().BeEmpty();
        }

        [Fact]
        public void ShouldRenderModalWithTitleAndBodyWhenVisible()
        {
            // given
            string randomTitle = GetRandomString();
            string randomBody = GetRandomString();

            // when
            IRenderedComponent<Modal> renderedModal =
                Render<Modal>(parameters => parameters
                    .Add(modal => modal.Visible, true)
                    .Add(modal => modal.Title, randomTitle)
                    .AddChildContent(randomBody));

            // then
            renderedModal.Find("div.modal").Should().NotBeNull();
            renderedModal.Find("div.modal-title").TextContent.Should().Contain(randomTitle);
            renderedModal.Find("div.modal-body").TextContent.Should().Contain(randomBody);
        }
    }
}
