// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Clients.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.RagStatuses.V2;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthStatusClientV2Tests
    {
        private readonly Mock<IRagStatusV2OrchestrationService> ragStatusV2OrchestrationServiceMock;
        private readonly IHealthStatusClientV2 healthV2Client;

        public HealthStatusClientV2Tests()
        {
            this.ragStatusV2OrchestrationServiceMock =
                new Mock<IRagStatusV2OrchestrationService>();

            this.healthV2Client =
                new HealthStatusClientV2(
                    ragStatusV2OrchestrationService:
                        this.ragStatusV2OrchestrationServiceMock.Object);
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
