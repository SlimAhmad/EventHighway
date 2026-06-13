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
    public partial class HealthV2ClientTests
    {
        private readonly Mock<IHealthV2CoordinationService> healthV2CoordinationServiceMock;
        private readonly IHealthV2Client healthV2Client;

        public HealthV2ClientTests()
        {
            this.healthV2CoordinationServiceMock =
                new Mock<IHealthV2CoordinationService>();

            this.healthV2Client =
                new HealthV2Client(
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

        private static IEnumerable<HealthCheckItemV2> CreateRandomHealthCheckItemV2s() =>
            Enumerable.Range(0, new IntRange(min: 2, max: 9).GetValue())
                .Select(_ => CreateRandomHealthCheckItemV2())
                    .ToList();

        private static HealthCheckItemV2 CreateRandomHealthCheckItemV2() =>
            new HealthCheckItemV2
            {
                Grouping = GetRandomString(),
                Item = GetRandomString(),
                Value = GetRandomString(),
                Description = GetRandomString(),
                StatusCode = 0,
                Status = HealthStatusV2.NA.ToString()
            };
    }
}
