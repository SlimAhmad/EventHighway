// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using EventHighway.Portal.Web.Components.Dashboard;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;
using EventHighway.Portal.Web.Models.Views.HealthDashboards.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

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

        [Fact]
        public void ShouldRenderTilesWhenLoaded()
        {
            // given
            List<HealthRagTile> randomTiles = CreateRandomRagTiles();

            this.healthViewServiceMock.Setup(service =>
                service.RetrieveHealthRagTilesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(randomTiles);

            // when
            IRenderedComponent<RagTileRow> renderedRagTileRow = Render<RagTileRow>();

            // then
            renderedRagTileRow.Instance.State.Should().Be(RagTileRowState.Content);
            renderedRagTileRow.FindAll("div.stat-tile").Should().HaveCount(randomTiles.Count);

            this.healthViewServiceMock.Verify(service =>
                service.RetrieveHealthRagTilesAsync(It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public void ShouldRenderErrorWhenViewServiceThrowsDependencyException()
        {
            // given
            var dependencyException =
                new HealthViewDependencyException(
                    innerException: new Xeption(message: GetRandomString()));

            this.healthViewServiceMock.Setup(service =>
                service.RetrieveHealthRagTilesAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            IRenderedComponent<RagTileRow> renderedRagTileRow = Render<RagTileRow>();

            // then
            renderedRagTileRow.Instance.State.Should().Be(RagTileRowState.Error);
            renderedRagTileRow.FindAll("div.alert-danger").Should().NotBeEmpty();
        }
    }
}
