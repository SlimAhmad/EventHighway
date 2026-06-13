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
    public partial class HealthV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveHealthSummaryV2Async()
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

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveHealthSummaryV2Async(randomCancellationToken))
                    .ReturnsAsync(returnedHealthCheckItemV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualHealthCheckItemV2s =
                await this.healthV2Client
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualHealthCheckItemV2s.Should()
                .BeEquivalentTo(expectedHealthCheckItemV2s);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveHealthSummaryV2Async(randomCancellationToken),
                    Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
