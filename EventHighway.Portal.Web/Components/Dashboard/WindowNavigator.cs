// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;

namespace EventHighway.Portal.Web.Components.Dashboard
{
    // Pure period/window math for the dashboard control bar. The coordination service derives the
    // window end from period + windowStart (Spec §4.2); this navigator only produces the inclusive
    // UTC window start for "current", "previous" and "next", the future-guard, and a human label.
    public static class WindowNavigator
    {
        public static DateTimeOffset Current(TrafficPeriodV2 period, DateTimeOffset now) =>
            throw new NotImplementedException();

        public static DateTimeOffset Previous(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            throw new NotImplementedException();

        public static DateTimeOffset Next(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            throw new NotImplementedException();

        public static bool CanGoNext(
            TrafficPeriodV2 period, DateTimeOffset windowStart, DateTimeOffset now) =>
            throw new NotImplementedException();

        public static string Label(TrafficPeriodV2 period, DateTimeOffset windowStart) =>
            throw new NotImplementedException();
    }
}
