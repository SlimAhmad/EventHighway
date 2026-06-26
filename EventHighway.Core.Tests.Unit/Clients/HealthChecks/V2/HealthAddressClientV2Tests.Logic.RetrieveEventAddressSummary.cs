// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthAddressClientV2Tests
    {
        [Fact]
        public async Task ShouldRetrieveEventAddressSummaryV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            TrafficPeriodV2 randomPeriod = GetRandomTrafficPeriodV2();
            DateTimeOffset randomWindowStart = GetRandomDateTimeOffset();

            IEnumerable<EventAddressSummaryV2> randomSummaries =
                CreateRandomEventAddressSummaryV2s();

            IEnumerable<EventAddressSummaryV2> returnedSummaries =
                randomSummaries;

            IEnumerable<EventAddressSummaryV2> expectedSummaries =
                returnedSummaries.DeepClone();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken))
                        .ReturnsAsync(returnedSummaries);

            // when
            IEnumerable<EventAddressSummaryV2> actualSummaries =
                await this.healthAddressClientV2
                    .RetrieveEventAddressSummaryV2Async(
                        randomPeriod, randomWindowStart, randomCancellationToken);

            // then
            actualSummaries.Should()
                .BeEquivalentTo(expectedSummaries);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    randomPeriod, randomWindowStart, randomCancellationToken),
                        Times.Once);

            this.healthV2CoordinationServiceMock
                .VerifyNoOtherCalls();
        }
    }
}
