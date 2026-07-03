// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.LoopDetections.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthLoopClientV2Tests
    {
        private readonly Mock<ILoopDetectionV2OrchestrationService> loopDetectionV2OrchestrationServiceMock;
        private readonly IHealthLoopClientV2 healthLoopClientV2;

        public HealthLoopClientV2Tests()
        {
            this.loopDetectionV2OrchestrationServiceMock =
                new Mock<ILoopDetectionV2OrchestrationService>();

            this.healthLoopClientV2 =
                new HealthLoopClientV2(
                    loopDetectionV2OrchestrationService:
                        this.loopDetectionV2OrchestrationServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static TrafficPeriodV2 GetRandomTrafficPeriodV2() =>
            TrafficPeriodV2.Day;

        private static LoopDetectionSummaryV2 CreateRandomLoopDetectionSummaryV2() =>
            new LoopDetectionSummaryV2
            {
                Period = GetRandomTrafficPeriodV2(),
                WindowStart = GetRandomDateTimeOffset(),
                WindowEnd = GetRandomDateTimeOffset(),
                WindowLabel = GetRandomString(),
                TotalActiveQuarantined = new IntRange(min: 2, max: 9).GetValue(),
                ByAddress = new List<LoopDetailV2>()
            };
    }
}
