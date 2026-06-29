// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Portal.Web.Components.Dashboard;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.Dashboard
{
    public class WindowNavigatorTests
    {
        private static readonly DateTimeOffset Now =
            new DateTimeOffset(2026, 6, 24, 13, 45, 0, TimeSpan.Zero); // Wed 24 Jun 2026

        [Fact]
        public void ShouldTruncateCurrentToPeriodStart()
        {
            WindowNavigator.Current(TrafficPeriodV2.Day, Now)
                .Should().Be(new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Current(TrafficPeriodV2.Week, Now)
                .Should().Be(new DateTimeOffset(2026, 6, 22, 0, 0, 0, TimeSpan.Zero)); // Monday

            WindowNavigator.Current(TrafficPeriodV2.Month, Now)
                .Should().Be(new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Current(TrafficPeriodV2.Year, Now)
                .Should().Be(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public void ShouldComputePreviousWindow()
        {
            var dayStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);

            WindowNavigator.Previous(TrafficPeriodV2.Day, dayStart)
                .Should().Be(new DateTimeOffset(2026, 6, 23, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Previous(TrafficPeriodV2.Week, dayStart)
                .Should().Be(new DateTimeOffset(2026, 6, 17, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Previous(TrafficPeriodV2.Month, dayStart)
                .Should().Be(new DateTimeOffset(2026, 5, 24, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Previous(TrafficPeriodV2.Year, dayStart)
                .Should().Be(new DateTimeOffset(2025, 6, 24, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public void ShouldComputeNextWindow()
        {
            var dayStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);

            WindowNavigator.Next(TrafficPeriodV2.Day, dayStart)
                .Should().Be(new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero));

            WindowNavigator.Next(TrafficPeriodV2.Month, dayStart)
                .Should().Be(new DateTimeOffset(2026, 7, 24, 0, 0, 0, TimeSpan.Zero));
        }

        [Fact]
        public void ShouldGuardFutureOnCanGoNext()
        {
            DateTimeOffset currentDay = WindowNavigator.Current(TrafficPeriodV2.Day, Now);

            // current window — cannot advance into the future
            WindowNavigator.CanGoNext(TrafficPeriodV2.Day, currentDay, Now).Should().BeFalse();

            // a past window — can advance
            DateTimeOffset previousDay = WindowNavigator.Previous(TrafficPeriodV2.Day, currentDay);
            WindowNavigator.CanGoNext(TrafficPeriodV2.Day, previousDay, Now).Should().BeTrue();
        }

        [Fact]
        public void ShouldFormatWindowLabel()
        {
            WindowNavigator.Label(TrafficPeriodV2.Month,
                new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero))
                .Should().Be("Jun 2026");

            WindowNavigator.Label(TrafficPeriodV2.Year,
                new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .Should().Be("2026");
        }
    }
}
