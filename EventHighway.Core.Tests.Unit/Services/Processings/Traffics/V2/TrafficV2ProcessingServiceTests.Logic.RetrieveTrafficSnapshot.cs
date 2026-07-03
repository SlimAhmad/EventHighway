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

namespace EventHighway.Core.Tests.Unit.Services.Processings.Traffics.V2
{
    public partial class TrafficV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveTrafficSnapshotV2ForDayWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero);

            var listenerEvents = new[]
            {
                CreateListenerEventV2WithDate(windowStart.AddMinutes(10), ListenerEventStatusV2.Success),
                CreateListenerEventV2WithDate(windowStart.AddMinutes(20), ListenerEventStatusV2.Success),
                CreateListenerEventV2WithDate(windowStart.AddHours(1), ListenerEventStatusV2.Error),
                CreateListenerEventV2WithDate(windowStart.AddHours(2), ListenerEventStatusV2.Pending),
                CreateListenerEventV2WithDate(windowStart.AddHours(3), ListenerEventStatusV2.Replay),
                CreateListenerEventV2WithDate(expectedWindowEnd.AddDays(1), ListenerEventStatusV2.Success)
            };

            var events = new[]
            {
                CreateEventV2WithDate(windowStart.AddMinutes(30), EventTypeV2.Immediate),
                CreateEventV2WithDate(windowStart.AddHours(1).AddMinutes(15), EventTypeV2.Scheduled),
                CreateEventV2WithDate(windowStart.AddDays(-1), EventTypeV2.Immediate),
                CreateEventV2WithDate(expectedWindowEnd.AddHours(1), EventTypeV2.Immediate)
            };

            events[0].ListenerEventV2s = listenerEvents;

            IQueryable<EventV2> retrievedEvents = events.AsQueryable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEvents);

            // when
            TrafficSnapshotV2 actualSnapshot =
                await this.trafficV2ProcessingService
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

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
