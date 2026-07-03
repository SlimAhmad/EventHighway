// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.AddressSummaries.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthAddressClientV2Tests
    {
        private readonly Mock<IAddressSummaryV2OrchestrationService> addressSummaryV2OrchestrationServiceMock;
        private readonly IHealthAddressClientV2 healthAddressClientV2;

        public HealthAddressClientV2Tests()
        {
            this.addressSummaryV2OrchestrationServiceMock =
                new Mock<IAddressSummaryV2OrchestrationService>();

            this.healthAddressClientV2 =
                new HealthAddressClientV2(
                    addressSummaryV2OrchestrationService:
                        this.addressSummaryV2OrchestrationServiceMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static TrafficPeriodV2 GetRandomTrafficPeriodV2() =>
            TrafficPeriodV2.Day;

        private static IEnumerable<EventAddressSummaryV2> CreateRandomEventAddressSummaryV2s() =>
            Enumerable.Range(0, new IntRange(min: 2, max: 9).GetValue())
                .Select(_ => new EventAddressSummaryV2
                {
                    EventAddressV2Id = Guid.NewGuid(),
                    Name = GetRandomString(),
                    WindowLabel = GetRandomString()
                })
                    .ToList();
    }
}
