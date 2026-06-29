// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Models.Views.HealthDashboards;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.HealthDashboards
{
    public partial class HealthViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveHealthRagTilesAsync()
        {
            // given
            List<HealthCheckItemV2> randomHealthCheckItems =
                CreateRandomHealthCheckItems();

            IEnumerable<HealthCheckItemV2> returnedHealthCheckItems =
                randomHealthCheckItems;

            List<HealthRagTile> expectedHealthRagTiles =
                MapToTiles(randomHealthCheckItems);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnedHealthCheckItems);

            // when
            List<HealthRagTile> actualHealthRagTiles =
                await this.healthViewService.RetrieveHealthRagTilesAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualHealthRagTiles.Should().BeEquivalentTo(expectedHealthRagTiles);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
