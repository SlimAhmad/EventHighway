// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2ForDayWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero);

            var events = new[]
            {
                CreateEventV2WithDate(windowStart.AddMinutes(30), EventTypeV2.Immediate),
                CreateEventV2WithDate(windowStart.AddHours(1).AddMinutes(15), EventTypeV2.Scheduled),
                CreateEventV2WithDate(windowStart.AddDays(-1), EventTypeV2.Immediate),
                CreateEventV2WithDate(expectedWindowEnd.AddHours(1), EventTypeV2.Immediate)
            }.AsQueryable();

            var listenerEvents = new[]
            {
                CreateListenerEventV2WithDate(windowStart.AddMinutes(10), ListenerEventStatusV2.Success),
                CreateListenerEventV2WithDate(windowStart.AddMinutes(20), ListenerEventStatusV2.Success),
                CreateListenerEventV2WithDate(windowStart.AddHours(1), ListenerEventStatusV2.Error),
                CreateListenerEventV2WithDate(windowStart.AddHours(2), ListenerEventStatusV2.Pending),
                CreateListenerEventV2WithDate(windowStart.AddHours(3), ListenerEventStatusV2.Replay),
                CreateListenerEventV2WithDate(expectedWindowEnd.AddDays(1), ListenerEventStatusV2.Success)
            }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(events);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(listenerEvents);

            // when
            TrafficSnapshotV2 actualSnapshot =
                await this.healthV2CoordinationService
                    .RetrieveTrafficSnapshotV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken);

            // then
            actualSnapshot.Period.Should().Be(TrafficPeriodV2.Day);
            actualSnapshot.WindowStart.Should().Be(windowStart);
            actualSnapshot.WindowEnd.Should().Be(expectedWindowEnd);
            actualSnapshot.WindowLabel.Should().Be("24 Jun 2026");
            actualSnapshot.TotalEvents.Should().Be(2);
            actualSnapshot.TotalListenerEvents.Should().Be(5);
            actualSnapshot.TotalSuccess.Should().Be(2);
            actualSnapshot.TotalErrors.Should().Be(1);
            actualSnapshot.TotalPending.Should().Be(1);
            actualSnapshot.TotalReplays.Should().Be(1);
            actualSnapshot.Buckets.Should().HaveCount(24);

            TrafficBucketV2 firstBucket = actualSnapshot.Buckets.ElementAt(0);
            firstBucket.Label.Should().Be("00:00");
            firstBucket.Events.Should().Be(1);
            firstBucket.ImmediateEvents.Should().Be(1);
            firstBucket.ListenerEvents.Should().Be(2);
            firstBucket.Success.Should().Be(2);

            TrafficBucketV2 secondBucket = actualSnapshot.Buckets.ElementAt(1);
            secondBucket.Label.Should().Be("01:00");
            secondBucket.ScheduledEvents.Should().Be(1);
            secondBucket.Errors.Should().Be(1);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2ForWeekWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2026, 6, 22, 0, 0, 0, TimeSpan.Zero);
            var emptyEvents = Array.Empty<EventV2>().AsQueryable();

            var listenerEvents = new[]
            {
                CreateListenerEventV2WithDate(windowStart.AddDays(2), ListenerEventStatusV2.Success)
            }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEvents);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(listenerEvents);

            // when
            TrafficSnapshotV2 actualSnapshot =
                await this.healthV2CoordinationService
                    .RetrieveTrafficSnapshotV2Async(
                        TrafficPeriodV2.Week, windowStart, randomCancellationToken);

            // then
            actualSnapshot.WindowEnd.Should().Be(expectedWindowEnd);
            actualSnapshot.WindowLabel.Should().Be("15 Jun – 21 Jun 2026");
            actualSnapshot.TotalListenerEvents.Should().Be(1);
            actualSnapshot.Buckets.Should().HaveCount(7);
            actualSnapshot.Buckets.ElementAt(2).ListenerEvents.Should().Be(1);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2ForMonthWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2026, 7, 1, 0, 0, 0, TimeSpan.Zero);
            var emptyEvents = Array.Empty<EventV2>().AsQueryable();

            var listenerEvents = new[]
            {
                CreateListenerEventV2WithDate(windowStart.AddDays(4), ListenerEventStatusV2.Success)
            }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEvents);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(listenerEvents);

            // when
            TrafficSnapshotV2 actualSnapshot =
                await this.healthV2CoordinationService
                    .RetrieveTrafficSnapshotV2Async(
                        TrafficPeriodV2.Month, windowStart, randomCancellationToken);

            // then
            actualSnapshot.WindowEnd.Should().Be(expectedWindowEnd);
            actualSnapshot.WindowLabel.Should().Be("Jun 2026");
            actualSnapshot.Buckets.Should().HaveCount(30);
            actualSnapshot.Buckets.ElementAt(0).Label.Should().Be("01");

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2ForYearWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var emptyEvents = Array.Empty<EventV2>().AsQueryable();

            var listenerEvents = new[]
            {
                CreateListenerEventV2WithDate(new DateTimeOffset(2026, 3, 10, 0, 0, 0, TimeSpan.Zero),
                    ListenerEventStatusV2.Success)
            }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEvents);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(listenerEvents);

            // when
            TrafficSnapshotV2 actualSnapshot =
                await this.healthV2CoordinationService
                    .RetrieveTrafficSnapshotV2Async(
                        TrafficPeriodV2.Year, windowStart, randomCancellationToken);

            // then
            actualSnapshot.WindowEnd.Should().Be(expectedWindowEnd);
            actualSnapshot.WindowLabel.Should().Be("2026");
            actualSnapshot.Buckets.Should().HaveCount(12);
            actualSnapshot.Buckets.ElementAt(2).Label.Should().Be("Mar");
            actualSnapshot.Buckets.ElementAt(2).ListenerEvents.Should().Be(1);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldSubstituteUtcNowWhenWindowStartIsMinValueOnRetrieveTrafficSnapshotAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset beforeCall = DateTimeOffset.UtcNow;
            var emptyEvents = Array.Empty<EventV2>().AsQueryable();
            var emptyListenerEvents = Array.Empty<ListenerEventV2>().AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEvents);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyListenerEvents);

            // when
            TrafficSnapshotV2 actualSnapshot =
                await this.healthV2CoordinationService
                    .RetrieveTrafficSnapshotV2Async(
                        TrafficPeriodV2.Day, DateTimeOffset.MinValue, randomCancellationToken);

            // then
            actualSnapshot.WindowStart.Should().BeOnOrAfter(beforeCall);
            actualSnapshot.WindowStart.Should().NotBe(DateTimeOffset.MinValue);
            actualSnapshot.WindowEnd.Should().Be(actualSnapshot.WindowStart.AddHours(24));
            actualSnapshot.Buckets.Should().HaveCount(24);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
