// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthAddressClientV2Tests
    {
        private readonly Mock<IHealthV2CoordinationService> healthV2CoordinationServiceMock;
        private readonly IHealthAddressClientV2 healthAddressClientV2;

        public HealthAddressClientV2Tests()
        {
            this.healthV2CoordinationServiceMock =
                new Mock<IHealthV2CoordinationService>();

            this.healthAddressClientV2 =
                new HealthAddressClientV2(
                    healthV2CoordinationService:
                        this.healthV2CoordinationServiceMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new HealthV2CoordinationValidationException(
                    someMessage,
                    someInnerException),

                new HealthV2CoordinationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
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
                    Id = Guid.NewGuid(),
                    Name = GetRandomString(),
                    WindowLabel = GetRandomString()
                })
                    .ToList();
    }
}
