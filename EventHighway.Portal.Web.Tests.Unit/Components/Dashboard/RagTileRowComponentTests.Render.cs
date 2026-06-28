// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using EventHighway.Portal.Web.Components.Dashboard;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Dashboard
{
    public partial class RagTileRowComponentTests
    {
        [Fact]
        public void ShouldInitializeComponentInLoadingState()
        {
            // given
            var pendingSource = new TaskCompletionSource<List<HealthRagTile>>();

            this.healthViewServiceMock.Setup(service =>
                service.RetrieveHealthRagTilesAsync(It.IsAny<CancellationToken>()))
                    .Returns(new ValueTask<List<HealthRagTile>>(pendingSource.Task));

            // when
            IRenderedComponent<RagTileRow> renderedRagTileRow = Render<RagTileRow>();

            // then
            renderedRagTileRow.Instance.State.Should().Be(RagTileRowState.Loading);
            renderedRagTileRow.FindAll("div.spinner-border").Should().NotBeEmpty();
        }
    }
}
