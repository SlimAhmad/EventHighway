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
    }
}
