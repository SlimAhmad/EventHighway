// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthStatusClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveHealthRagStatusV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<HealthCheckItemV2> randomHealthCheckItemV2s =
                CreateRandomHealthCheckItemV2s();

            IEnumerable<HealthCheckItemV2> returnedHealthCheckItemV2s =
                randomHealthCheckItemV2s;

            IEnumerable<HealthCheckItemV2> expectedHealthCheckItemV2s =
                returnedHealthCheckItemV2s.DeepClone();

            this.ragStatusV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthRagStatusV2Async(randomCancellationToken))
                    .ReturnsAsync(returnedHealthCheckItemV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualHealthCheckItemV2s =
                await this.healthV2Client
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualHealthCheckItemV2s.Should()
                .BeEquivalentTo(expectedHealthCheckItemV2s);

            this.ragStatusV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthRagStatusV2Async(randomCancellationToken),
                    Times.Once);

            this.ragStatusV2OrchestrationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
