// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.LoopDetections.V2
{
    public partial class LoopDetectionV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveLoopDetectionSummaryV2ForWindowAsync()
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
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(1), "h1", 5, EventStatusV2.Quarantined),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(2), "h2", 5, EventStatusV2.Quarantined),
                CreateEventV2ForAddress(addressBId, windowStart.AddHours(3), "h3", 5, EventStatusV2.Quarantined),
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(4), "h4", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, windowStart.AddDays(-1), "h5", 5, EventStatusV2.Quarantined)
            }.AsQueryable();

            var archivedEvents = new[]
            {
                CreateEventArchiveV2ForAddress(addressAId, windowStart.AddHours(6), EventArchiveStatusV2.Quarantined),
                CreateEventArchiveV2ForAddress(addressBId, windowStart.AddHours(7), EventArchiveStatusV2.Active),
                CreateEventArchiveV2ForAddress(addressAId, windowStart.AddDays(-2), EventArchiveStatusV2.Quarantined)
            }.AsQueryable();

            SetupLoopDetectionFoundationMocks(
                randomCancellationToken, addresses, events, archivedEvents);

            // when
            LoopDetectionSummaryV2 actualSummary =
                await this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken);

            // then
            actualSummary.Period.Should().Be(TrafficPeriodV2.Day);
            actualSummary.WindowStart.Should().Be(windowStart);
            actualSummary.WindowEnd.Should().Be(expectedWindowEnd);
            actualSummary.WindowLabel.Should().Be("24 Jun 2026");
            actualSummary.TotalActiveQuarantined.Should().Be(3);
            actualSummary.TotalArchivedQuarantined.Should().Be(1);
            actualSummary.TotalInWindow.Should().Be(4);
            actualSummary.ByAddress.Should().HaveCount(2);

            actualSummary.ByAddress.First().EventAddressV2Id.Should().Be(addressAId);

            LoopDetailV2 detailA =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressAId);

            detailA.EventAddressV2Name.Should().Be(addressAName);
            detailA.ActiveQuarantined.Should().Be(2);
            detailA.ArchivedQuarantined.Should().Be(1);
            detailA.InWindow.Should().Be(3);
            detailA.MostRecentDetection.Should().Be(windowStart.AddHours(6));
            detailA.Status.Should().Be(HealthStatusV2.Amber);

            LoopDetailV2 detailB =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressBId);

            detailB.EventAddressV2Name.Should().Be(addressBName);
            detailB.ActiveQuarantined.Should().Be(1);
            detailB.ArchivedQuarantined.Should().Be(0);
            detailB.InWindow.Should().Be(1);
            detailB.MostRecentDetection.Should().Be(windowStart.AddHours(3));

            VerifyLoopDetectionFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldSplitLoopDetectionSummaryV2ByParticipantForWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            Guid addressId = Guid.NewGuid();
            string addressName = GetRandomString();

            Guid participantAId = Guid.NewGuid();
            Guid participantBId = Guid.NewGuid();
            string participantAName = GetRandomString();
            string participantBName = GetRandomString();

            EventParticipantV2 participantA = CreateEventParticipantV2(
                participantAId, participantAName, GetRandomString(), GetRandomString(), true);

            EventParticipantV2 participantB = CreateEventParticipantV2(
                participantBId, participantBName, GetRandomString(), GetRandomString(), true);

            var addresses = new[]
            {
                CreateEventAddressV2(addressId, addressName, GetRandomString())
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForParticipant(Guid.NewGuid(), addressId, participantAId, participantA,
                    windowStart.AddHours(1), "a1", EventStatusV2.Quarantined),
                CreateEventV2ForParticipant(Guid.NewGuid(), addressId, participantAId, participantA,
                    windowStart.AddHours(2), "a2", EventStatusV2.Quarantined),
                CreateEventV2ForParticipant(Guid.NewGuid(), addressId, participantBId, participantB,
                    windowStart.AddHours(3), "b1", EventStatusV2.Quarantined)
            }.AsQueryable();

            var emptyArchivedEvents = Array.Empty<EventArchiveV2>().AsQueryable();

            SetupLoopDetectionFoundationMocks(
                randomCancellationToken, addresses, events, emptyArchivedEvents);

            // when
            LoopDetectionSummaryV2 actualSummary =
                await this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken);

            // then
            actualSummary.ByAddress.Should().HaveCount(2);

            LoopDetailV2 detailA =
                actualSummary.ByAddress.Single(d => d.EventParticipantV2Id == participantAId);

            detailA.EventAddressV2Id.Should().Be(addressId);
            detailA.EventAddressV2Name.Should().Be(addressName);
            detailA.EventParticipantV2Name.Should().Be(participantAName);
            detailA.ActiveQuarantined.Should().Be(2);
            detailA.ArchivedQuarantined.Should().Be(0);
            detailA.InWindow.Should().Be(2);
            detailA.MostRecentDetection.Should().Be(windowStart.AddHours(2));

            LoopDetailV2 detailB =
                actualSummary.ByAddress.Single(d => d.EventParticipantV2Id == participantBId);

            detailB.EventParticipantV2Name.Should().Be(participantBName);
            detailB.ActiveQuarantined.Should().Be(1);
            detailB.InWindow.Should().Be(1);
            detailB.MostRecentDetection.Should().Be(windowStart.AddHours(3));

            VerifyLoopDetectionFoundationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnEmptyByAddressLoopDetectionSummaryV2WhenNoQuarantineInWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            Guid addressAId = Guid.NewGuid();

            var addresses = new[]
            {
                CreateEventAddressV2(addressAId, GetRandomString(), GetRandomString())
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForAddress(addressAId, windowStart.AddHours(1), "h1", 5, EventStatusV2.Active)
            }.AsQueryable();

            var emptyArchivedEvents = Array.Empty<EventArchiveV2>().AsQueryable();

            SetupLoopDetectionFoundationMocks(
                randomCancellationToken, addresses, events, emptyArchivedEvents);

            // when
            LoopDetectionSummaryV2 actualSummary =
                await this.loopDetectionV2OrchestrationService
                    .RetrieveLoopDetectionSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken);

            // then
            actualSummary.TotalActiveQuarantined.Should().Be(0);
            actualSummary.TotalArchivedQuarantined.Should().Be(0);
            actualSummary.TotalInWindow.Should().Be(0);
            actualSummary.ByAddress.Should().BeEmpty();

            VerifyLoopDetectionFoundationMocksOnce(randomCancellationToken);
        }
    }
}
