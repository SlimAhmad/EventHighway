// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.RetrySummaries.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthRetryClientV2Tests
    {
        private readonly Mock<IRetrySummaryV2OrchestrationService> retrySummaryV2OrchestrationServiceMock;
        private readonly IHealthRetryClientV2 healthRetryClientV2;

        public HealthRetryClientV2Tests()
        {
            this.retrySummaryV2OrchestrationServiceMock =
                new Mock<IRetrySummaryV2OrchestrationService>();

            this.healthRetryClientV2 =
                new HealthRetryClientV2(
                    retrySummaryV2OrchestrationService:
                        this.retrySummaryV2OrchestrationServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static RetryHealthSummaryV2 CreateRandomRetryHealthSummaryV2() =>
            new RetryHealthSummaryV2
            {
                TotalActiveEvents = new IntRange(min: 2, max: 9).GetValue(),
                DeadEvents = new IntRange(min: 0, max: 5).GetValue(),
                CriticalEvents = new IntRange(min: 0, max: 5).GetValue(),
                HealthyEvents = new IntRange(min: 0, max: 5).GetValue(),
                Distribution = new List<RetryBucketV2>(),
                ByAddress = new List<RetryAddressDetailV2>()
            };
    }
}
