// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.DuplicateSummaries.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthDuplicateClientV2Tests
    {
        private readonly Mock<IDuplicateSummaryV2OrchestrationService> duplicateSummaryV2OrchestrationServiceMock;
        private readonly IHealthDuplicateClientV2 healthDuplicateClientV2;

        public HealthDuplicateClientV2Tests()
        {
            this.duplicateSummaryV2OrchestrationServiceMock =
                new Mock<IDuplicateSummaryV2OrchestrationService>();

            this.healthDuplicateClientV2 =
                new HealthDuplicateClientV2(
                    duplicateSummaryV2OrchestrationService:
                        this.duplicateSummaryV2OrchestrationServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static TrafficPeriodV2 GetRandomTrafficPeriodV2() =>
            TrafficPeriodV2.Day;

        private static DuplicateDetectionSummaryV2 CreateRandomDuplicateDetectionSummaryV2() =>
            new DuplicateDetectionSummaryV2
            {
                Period = GetRandomTrafficPeriodV2(),
                WindowStart = GetRandomDateTimeOffset(),
                WindowEnd = GetRandomDateTimeOffset(),
                WindowLabel = GetRandomString(),
                TotalDuplicatesDetected = new IntRange(min: 2, max: 9).GetValue(),
                ByAddress = new List<DuplicateDetailV2>()
            };
    }
}
