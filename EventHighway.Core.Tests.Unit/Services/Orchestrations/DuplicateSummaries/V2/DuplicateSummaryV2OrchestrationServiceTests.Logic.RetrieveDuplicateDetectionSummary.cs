// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.DuplicateSummaries.V2
{
    public partial class DuplicateSummaryV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveDuplicateDetectionSummaryV2ForWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero);
            Guid addressAId = Guid.NewGuid();
            Guid addressBId = Guid.NewGuid();
            string addressAName = GetRandomString();
            string addressBName = GetRandomString();

            var addresses = new[]
            {
                CreateEventAddressV2(addressAId, addressAName, GetRandomString()),
                CreateEventAddressV2(addressBId, addressBName, GetRandomString())
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(1), "h1", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(2), "h1", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(3), "h1", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(4), "h2", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressBId, windowStart.AddHours(1), "b1", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressBId, windowStart.AddHours(5), "b1", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddDays(-1), "h1", 5, EventStatusV2.Active)
            }.AsQueryable();

            SetupDuplicateFoundationMocks(randomCancellationToken, addresses, events);

            // when
            DuplicateDetectionSummaryV2 actualSummary =
                await this.duplicateSummaryV2OrchestrationService
                    .RetrieveDuplicateDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken);

            // then
            actualSummary.Period.Should().Be(TrafficPeriodV2.Day);
            actualSummary.WindowStart.Should().Be(windowStart);
            actualSummary.WindowEnd.Should().Be(expectedWindowEnd);
            actualSummary.WindowLabel.Should().Be("24 Jun 2026");
            actualSummary.TotalDuplicatesDetected.Should().Be(3);
            actualSummary.TotalUniqueEvents.Should().Be(3);
            actualSummary.OverallDuplicateRate.Should().Be(50m);
            actualSummary.ByAddress.Should().HaveCount(2);
            actualSummary.ByAddress.First().EventAddressV2Id.Should().Be(addressAId);

            DuplicateDetailV2 detailA =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressAId);

            detailA.EventAddressV2Name.Should().Be(addressAName);
            detailA.EventParticipantV2Id.Should().BeNull();
            detailA.EventParticipantV2Name.Should().Be("Unknown");
            detailA.TotalEvents.Should().Be(4);
            detailA.Duplicates.Should().Be(2);
            detailA.DuplicateRate.Should().Be(50m);
            detailA.LastDuplicateSeen.Should().Be(windowStart.AddHours(3));

            DuplicateDetailV2 detailB =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressBId);

            detailB.TotalEvents.Should().Be(2);
            detailB.Duplicates.Should().Be(1);
            detailB.LastDuplicateSeen.Should().Be(windowStart.AddHours(5));

            VerifyDuplicateFoundationMocksOnce(randomCancellationToken);
        }
    }
}
